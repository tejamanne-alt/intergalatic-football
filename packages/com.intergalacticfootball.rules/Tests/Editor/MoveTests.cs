using NUnit.Framework;

namespace IntergalacticFootball.Rules.Tests
{
    public class MoveTests
    {
        [Test]
        public void OffTheBallZonesDoNotMatter()
        {
            var s = T.Empty();
            T.Put(s, T.Home(T.R), 5, 4);
            T.Put(s, T.Away(T.A), 6, 5);
            var result = MatchEngine.Apply(s, GameAction.Move(T.Home(T.R), T.Path(6, 4, 7, 4, 8, 4)));
            Assert.That(result.Contests(), Is.Empty);
            Assert.That(s.GetPlayer(T.Home(T.R)).Pos, Is.EqualTo(new Tile(8, 4)));
        }

        [Test]
        public void IllegalPathsAreRejected()
        {
            var s = T.Empty();
            T.Put(s, T.Home(T.S), 5, 4); // Testers striker: Pace 6, moves 3
            T.Put(s, T.Home(T.P), 6, 4);
            T.Put(s, T.Home(T.K), 1, 4);
            int striker = T.Home(T.S);
            Assert.That(MatchEngine.Validate(s, GameAction.Move(striker, T.Path(6, 4))), Does.Contain("taken"));
            Assert.That(MatchEngine.Validate(s, GameAction.Move(striker, T.Path(5, 5, 5, 6, 5, 7, 5, 8))), Does.Contain("at most 3"));
            Assert.That(MatchEngine.Validate(s, GameAction.Move(striker, T.Path(5, 6))), Does.Contain("neighbouring"));
            Assert.That(MatchEngine.Validate(s, GameAction.Move(T.Home(T.K), T.Path(2, 4, 3, 4))), Does.Contain("penalty area"));
            Assert.That(MatchEngine.Validate(s, GameAction.Move(striker, new Tile[0])), Does.Contain("path"));
        }

        [Test]
        public void SteppingIntoAZoneWithTheBallIsAContestAndEndsTheMove()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.R), 4, 4); // Testers runner: Pace 8, moves 3
            T.Put(s, T.Away(T.A), 8, 4);         // Testers anchor: Tackle 7
            var carrier = s.GetPlayer(T.Home(T.R));
            var defender = s.GetPlayer(T.Away(T.A));

            Assert.That(MatchEngine.Validate(s, GameAction.Move(carrier.Id, T.Path(5, 4, 6, 4, 7, 4))), Is.Null);

            var result = T.Succeed(s, GameAction.Move(carrier.Id, T.Path(5, 4, 6, 4, 7, 4)));
            var contests = result.Contests();
            Assert.That(contests, Has.Count.EqualTo(1));
            Assert.That(contests[0].ContestKind, Is.EqualTo(ContestKind.Dribble));
            Assert.That(contests[0].Chance, Is.EqualTo(Rules.Contests.Chance(s.Config, carrier.Stats.Pace, defender.Stats.Tackle, 0)));
            Assert.That(carrier.Pos, Is.EqualTo(new Tile(7, 4)));
            Assert.That(s.Carrier, Is.SameAs(carrier));
        }

        [Test]
        public void ACarrierCannotPlanAPathThroughAZone()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.R), 4, 4);
            T.Put(s, T.Away(T.A), 7, 5);
            var error = MatchEngine.Validate(s, GameAction.Move(T.Home(T.R), T.Path(5, 4, 6, 4, 7, 3)));
            Assert.That(error, Does.Contain("ends the move"));
        }

        [Test]
        public void LosingADribbleHandsTheDefenderTheBall()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.R), 5, 4);
            T.Put(s, T.Away(T.A), 7, 4);
            var result = T.Fail(s, GameAction.Move(T.Home(T.R), T.Path(6, 4)));
            Assert.That(result.Succeeded, Is.False);
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Away(T.A))));
            Assert.That(result.Events(EventType.Dispossessed), Has.Count.EqualTo(1));
            Assert.That(s.Team(Side.Away).Flux, Is.EqualTo(s.Config.FluxPerTackle));
        }

        [Test]
        public void EveryDefenderAroundTheTileContests()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.R), 5, 4);
            T.Put(s, T.Away(T.A), 7, 3);
            T.Put(s, T.Away(T.B), 7, 5);
            var result = T.Succeed(s, GameAction.Move(T.Home(T.R), T.Path(6, 4)));
            Assert.That(result.Contests(), Has.Count.EqualTo(2));
        }

        [Test]
        public void AnchoredCarriersKeepTheBallWhenBeaten()
        {
            var s = T.Empty(Catalog.Gravions, T.Testers);
            T.PutWithBall(s, T.Home(T.R), 5, 4);
            T.Put(s, T.Away(T.A), 7, 4);
            var result = T.Fail(s, GameAction.Move(T.Home(T.R), T.Path(6, 4)));
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Home(T.R))));
            Assert.That(result.Events(EventType.HeldOn), Has.Count.EqualTo(1));
            Assert.That(s.Team(Side.Away).Flux, Is.EqualTo(0));
        }

        [Test]
        public void PhasingIgnoresTheFirstZoneEachMove()
        {
            var s = T.Empty(Catalog.Blinkers, T.Testers);
            T.PutWithBall(s, T.Home(T.R), 3, 4); // Blinker runner moves 4
            T.Put(s, T.Away(T.A), 5, 5);         // zone covers (4,4) and (5,4)
            T.Put(s, T.Away(T.B), 8, 4);         // zone covers (7,4)
            var path = T.Path(4, 4, 5, 4, 6, 4, 7, 4);
            Assert.That(MatchEngine.Validate(s, GameAction.Move(T.Home(T.R), path)), Is.Null);
            var result = T.Succeed(s, GameAction.Move(T.Home(T.R), path));
            Assert.That(result.Events(EventType.ZoneIgnored), Has.Count.EqualTo(1));
            Assert.That(result.Contests(), Has.Count.EqualTo(1));
            Assert.That(result.Contests()[0].OtherPlayerId, Is.EqualTo(T.Away(T.B)));
        }

        [Test]
        public void WalkingOntoTheLooseBallPicksItUpAndEndsTheMove()
        {
            var s = T.Empty();
            T.Put(s, T.Home(T.R), 5, 4);
            Scenario.PlaceBall(s, new Tile(6, 4));
            Assert.That(MatchEngine.Validate(s, GameAction.Move(T.Home(T.R), T.Path(6, 4, 7, 4))), Does.Contain("ends the move"));
            var result = MatchEngine.Apply(s, GameAction.Move(T.Home(T.R), T.Path(6, 4)));
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Home(T.R))));
            Assert.That(result.Events(EventType.PickedUp), Has.Count.EqualTo(1));
        }

        [Test]
        public void PathingFindsTheRouteAroundAZone()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.R), 4, 4);
            T.Put(s, T.Away(T.A), 6, 5);
            var plan = Pathing.Find(s, s.GetPlayer(T.Home(T.R)), new Tile(7, 2));
            Assert.That(plan, Is.Not.Null);
            Assert.That(plan.SuccessProbability, Is.EqualTo(1.0));
            Assert.That(plan.Contests, Is.EqualTo(0));
            Assert.That(MatchEngine.Validate(s, GameAction.Move(T.Home(T.R), plan.Path)), Is.Null);
        }

        [Test]
        public void EveryPathingPlanIsLegal()
        {
            var s = T.NewMatch(Catalog.Gravions, Catalog.Blinkers);
            foreach (var p in s.PlayersOf(Side.Home))
            foreach (var plan in Pathing.Reachable(s, p))
                Assert.That(MatchEngine.Validate(s, GameAction.Move(p.Id, plan.Path)), Is.Null, $"{p.Label} to {plan.Destination}");
        }
    }
}
