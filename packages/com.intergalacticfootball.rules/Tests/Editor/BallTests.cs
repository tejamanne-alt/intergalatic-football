using NUnit.Framework;

namespace IntergalacticFootball.Rules.Tests
{
    public class PassTests
    {
        [Test]
        public void AnOpenPassAlwaysArrivesAndEarnsFlux()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.P), 5, 4);
            T.Put(s, T.Home(T.Q), 8, 4);
            var pass = GameAction.Pass(T.Home(T.P), new Tile(8, 4));
            var preview = MatchEngine.Preview(s, pass);
            Assert.That(preview.Contests, Is.Empty);
            Assert.That(preview.SuccessPercent, Is.EqualTo(100));

            var result = MatchEngine.Apply(s, pass);
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Home(T.Q))));
            Assert.That(result.Events(EventType.PassCompleted), Has.Count.EqualTo(1));
            Assert.That(s.Team(Side.Home).Flux, Is.EqualTo(s.Config.FluxPerPass));
        }

        [Test]
        public void APassThroughAZoneCanBeIntercepted()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.P), 5, 4); // Pass 8
            T.Put(s, T.Home(T.Q), 9, 4);
            T.Put(s, T.Away(T.A), 7, 5);         // Tackle 7, marks the lane
            var pass = GameAction.Pass(T.Home(T.P), new Tile(9, 4));

            var contests = MatchEngine.Preview(s, pass).Contests;
            Assert.That(contests, Has.Count.EqualTo(1));
            Assert.That(contests[0].Kind, Is.EqualTo(ContestKind.Interception));
            Assert.That(contests[0].Chance, Is.EqualTo(50 + 8 * (8 - 7) + s.Config.LaneAdjacentModifier));

            var result = T.Fail(s, pass);
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Away(T.A))));
            Assert.That(result.Events(EventType.Intercepted), Has.Count.EqualTo(1));
            Assert.That(s.Team(Side.Away).Flux, Is.EqualTo(s.Config.FluxPerTackle));
        }

        [Test]
        public void ADefenderStandingInTheLaneIsHarderToBeat()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.P), 5, 4);
            T.Put(s, T.Home(T.Q), 9, 4);
            T.Put(s, T.Away(T.A), 7, 4);
            var contests = MatchEngine.Preview(s, GameAction.Pass(T.Home(T.P), new Tile(9, 4))).Contests;
            Assert.That(contests[0].Chance, Is.EqualTo(50 + 8 * (8 - 7) + s.Config.LaneOnPathModifier));
        }

        [Test]
        public void EachDefenderContestsOnceAndAMarkedReceiverCounts()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.P), 3, 4);
            T.Put(s, T.Home(T.Q), 9, 4);
            T.Put(s, T.Away(T.A), 5, 3);  // zone covers (4,4), (5,4), (6,4)
            T.Put(s, T.Away(T.B), 10, 5); // marks the receiver
            var contests = MatchEngine.Preview(s, GameAction.Pass(T.Home(T.P), new Tile(9, 4))).Contests;
            Assert.That(contests, Has.Count.EqualTo(2));
            Assert.That(contests[0].DefenderId, Is.EqualTo(T.Away(T.A)));
            Assert.That(contests[1].DefenderId, Is.EqualTo(T.Away(T.B)));
        }

        [Test]
        public void LongPassesAreHarderToGetThrough()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.P), 2, 4);
            T.Put(s, T.Home(T.Q), 9, 4); // 7 tiles
            T.Put(s, T.Away(T.A), 6, 5);
            var c = MatchEngine.Preview(s, GameAction.Pass(T.Home(T.P), new Tile(9, 4))).Contests[0];
            int longPass = -s.Config.LongPassPenaltyPerTile * (7 - s.Config.LongPassFreeDistance);
            Assert.That(c.Chance, Is.EqualTo(50 + 8 + s.Config.LaneAdjacentModifier + longPass));
        }

        [Test]
        public void PassesHaveARangeAndCannotTargetOpponents()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.P), 2, 4); // Pass 8: range 8
            T.Put(s, T.Away(T.A), 5, 4);
            Assert.That(MatchEngine.Validate(s, GameAction.Pass(T.Home(T.P), new Tile(11, 4))), Does.Contain("range"));
            Assert.That(MatchEngine.Validate(s, GameAction.Pass(T.Home(T.P), new Tile(5, 4))), Does.Contain("standing there"));
            Assert.That(MatchEngine.Validate(s, GameAction.Pass(T.Home(T.Q), new Tile(5, 5))), Does.Contain("not on the pitch"));
        }

        [Test]
        public void APassIntoSpaceLeavesTheBallLoose()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.P), 5, 4);
            var result = MatchEngine.Apply(s, GameAction.Pass(T.Home(T.P), new Tile(9, 2)));
            Assert.That(s.BallIsLoose, Is.True);
            Assert.That(s.LooseBallTile, Is.EqualTo(new Tile(9, 2)));
            Assert.That(result.Events(EventType.PassIntoSpace), Has.Count.EqualTo(1));
            Assert.That(s.Team(Side.Home).Flux, Is.EqualTo(0));
        }

        [Test]
        public void PreviewMultipliesContestsAndChangesNothing()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.P), 3, 4);
            T.Put(s, T.Home(T.Q), 9, 4);
            T.Put(s, T.Away(T.A), 5, 3);
            T.Put(s, T.Away(T.B), 10, 5);
            var before = s.Clone();

            var preview = MatchEngine.Preview(s, GameAction.Pass(T.Home(T.P), new Tile(9, 4)));
            double expected = preview.Contests[0].Chance / 100.0 * (preview.Contests[1].Chance / 100.0);
            Assert.That(preview.SuccessProbability, Is.EqualTo(expected).Within(1e-9));

            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Home(T.P))));
            Assert.That(s.ActionsLeft, Is.EqualTo(3));
            Assert.That(s.Rng.NextUInt(), Is.EqualTo(before.Rng.NextUInt()), "preview advanced the roll stream");
        }
    }

    public class ShotTests
    {
        static MatchState ShootingPosition(out Player striker, out Player keeper)
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.S), 12, 4); // Shot 8, 3 tiles from goal
            T.Put(s, T.Away(T.K), 14, 4);         // Tackle 8
            striker = s.GetPlayer(T.Home(T.S));
            keeper = s.GetPlayer(T.Away(T.K));
            return s;
        }

        [Test]
        public void SaveChanceComesFromShotKeeperAndDistance()
        {
            var s = ShootingPosition(out var striker, out var keeper);
            var contests = MatchEngine.Preview(s, GameAction.Shoot(striker.Id, 4)).Contests;
            Assert.That(contests, Has.Count.EqualTo(1));
            Assert.That(contests[0].Kind, Is.EqualTo(ContestKind.Save));
            Assert.That(contests[0].Chance, Is.EqualTo(Rules.Contests.Chance(s.Config, striker.Stats.Shot, keeper.Stats.Tackle, s.Config.KeeperSaveModifier)));
        }

        [Test]
        public void AGoalCountsAndTheConcedingSideKicksOff()
        {
            var s = ShootingPosition(out var striker, out _);
            var result = T.Succeed(s, GameAction.Shoot(striker.Id, 4));
            Assert.That(result.Succeeded, Is.True);
            Assert.That(s.Team(Side.Home).Score, Is.EqualTo(1));
            Assert.That(result.Events(EventType.Goal), Has.Count.EqualTo(1));
            Assert.That(s.ActiveSide, Is.EqualTo(Side.Away));
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Away(T.S))));
            Assert.That(s.BallTile, Is.EqualTo(Pitch.Centre));
        }

        [Test]
        public void ASaveGivesTheKeeperTheBallAndFlux()
        {
            var s = ShootingPosition(out var striker, out var keeper);
            var result = T.Fail(s, GameAction.Shoot(striker.Id, 4));
            Assert.That(s.Carrier, Is.SameAs(keeper));
            Assert.That(result.Events(EventType.Saved), Has.Count.EqualTo(1));
            Assert.That(s.Team(Side.Away).Flux, Is.EqualTo(s.Config.FluxPerSave));
            Assert.That(s.ActiveSide, Is.EqualTo(Side.Home), "a save doesn't end the shooter's turn");
            Assert.That(s.ActionsLeft, Is.EqualTo(2));
        }

        [Test]
        public void AStretchedKeeperIsEasierToBeat()
        {
            var s = ShootingPosition(out var striker, out var keeper);
            Scenario.Place(s, keeper.Id, new Tile(14, 5));
            var c = MatchEngine.Preview(s, GameAction.Shoot(striker.Id, 3)).Contests[0];
            Assert.That(c.Kind, Is.EqualTo(ContestKind.Save));
            Assert.That(c.Chance, Is.EqualTo(Rules.Contests.Chance(s.Config, 8, 8, s.Config.KeeperSaveModifier + s.Config.KeeperStretchedModifier)));
        }

        [Test]
        public void WithTheKeeperOutOfReachOnlyAccuracyMatters()
        {
            var s = ShootingPosition(out var striker, out var keeper);
            Scenario.Place(s, keeper.Id, new Tile(12, 7));
            Scenario.Place(s, striker.Id, new Tile(10, 3)); // 5 tiles from goal
            var c = MatchEngine.Preview(s, GameAction.Shoot(striker.Id, 3)).Contests[0];
            Assert.That(c.Kind, Is.EqualTo(ContestKind.Accuracy));
            Assert.That(c.Chance, Is.EqualTo(s.Config.UnguardedAccuracy - 2 * s.Config.UnguardedPenaltyPerTile));

            var result = T.Fail(s, GameAction.Shoot(striker.Id, 3));
            Assert.That(result.Events(EventType.Missed), Has.Count.EqualTo(1));
            Assert.That(s.Carrier, Is.SameAs(keeper), "goal kick");
        }

        [Test]
        public void OutfieldDefendersCanBlockButTheKeeperOnlySaves()
        {
            var s = ShootingPosition(out var striker, out var keeper);
            T.Put(s, T.Away(T.A), 13, 4);
            var contests = MatchEngine.Preview(s, GameAction.Shoot(striker.Id, 4)).Contests;
            Assert.That(contests, Has.Count.EqualTo(2));
            Assert.That(contests[0].Kind, Is.EqualTo(ContestKind.Block));
            Assert.That(contests[0].Chance, Is.EqualTo(Rules.Contests.Chance(s.Config, 8, 7, s.Config.ShotLaneOnPathModifier)));
            Assert.That(contests[1].Kind, Is.EqualTo(ContestKind.Save));

            var result = T.Fail(s, GameAction.Shoot(striker.Id, 4));
            Assert.That(result.Events(EventType.Blocked), Has.Count.EqualTo(1));
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Away(T.A))));
        }

        [Test]
        public void ShotsHaveARange()
        {
            var s = ShootingPosition(out var striker, out _);
            Scenario.Place(s, striker.Id, new Tile(6, 4)); // 9 tiles, range 7
            Assert.That(MatchEngine.Validate(s, GameAction.Shoot(striker.Id, 4)), Does.Contain("Out of range"));
            Assert.That(MatchEngine.Validate(s, GameAction.Shoot(striker.Id, 7)), Does.Contain("row"));
        }
    }

    public class TackleTests
    {
        [Test]
        public void TacklingNeedsAnAdjacentOpponentWithTheBall()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Away(T.S), 6, 4);
            T.Put(s, T.Home(T.A), 4, 4);
            T.Put(s, T.Home(T.B), 5, 5);
            Assert.That(MatchEngine.Validate(s, GameAction.Tackle(T.Home(T.A))), Does.Contain("next to"));
            Assert.That(MatchEngine.Validate(s, GameAction.Tackle(T.Home(T.B))), Is.Null);
            Scenario.GiveBall(s, T.Home(T.A));
            Assert.That(MatchEngine.Validate(s, GameAction.Tackle(T.Home(T.B))), Does.Contain("No opponent"));
        }

        [Test]
        public void AWonTackleTakesTheBallAndEarnsFlux()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Away(T.S), 6, 4); // Pace 6
            T.Put(s, T.Home(T.B), 5, 5);         // Tackle 7
            var tackle = GameAction.Tackle(T.Home(T.B));
            Assert.That(MatchEngine.Preview(s, tackle).Contests[0].Chance, Is.EqualTo(50 + 8 * (7 - 6)));
            var result = T.Succeed(s, tackle);
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Home(T.B))));
            Assert.That(result.Events(EventType.TackleWon), Has.Count.EqualTo(1));
            Assert.That(s.Team(Side.Home).Flux, Is.EqualTo(s.Config.FluxPerTackle));
        }

        [Test]
        public void AFailedTackleChangesNothing()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Away(T.S), 6, 4);
            T.Put(s, T.Home(T.B), 5, 5);
            var result = T.Fail(s, GameAction.Tackle(T.Home(T.B)));
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Away(T.S))));
            Assert.That(result.Events(EventType.TackleFailed), Has.Count.EqualTo(1));
            Assert.That(s.Team(Side.Home).Flux, Is.EqualTo(0));
        }

        [Test]
        public void FluxIsCappedAtTheMaximum()
        {
            var s = T.Empty();
            Scenario.SetFlux(s, Side.Home, s.Config.MaxFlux);
            T.PutWithBall(s, T.Home(T.P), 5, 4);
            T.Put(s, T.Home(T.Q), 8, 4);
            var result = MatchEngine.Apply(s, GameAction.Pass(T.Home(T.P), new Tile(8, 4)));
            Assert.That(s.Team(Side.Home).Flux, Is.EqualTo(s.Config.MaxFlux));
            Assert.That(result.Events(EventType.FluxGained), Is.Empty);
        }
    }
}
