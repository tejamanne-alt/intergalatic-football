using NUnit.Framework;

namespace IntergalacticFootball.Rules.Tests
{
    public class PowerTests
    {
        // Gravions loadout: K Gravity Well, A/B Shockwave, P/Q Gravity Well, R Shockwave, S Meteor Strike.
        // Blinkers loadout: K Blink, A/B Rift Tackle, P/Q Phase Pass, R/S Blink.

        [Test]
        public void PowersCostFluxAndGoOnCooldown()
        {
            var s = T.Empty(Catalog.Blinkers, Catalog.Gravions);
            Scenario.SetFlux(s, Side.Home, 5);
            T.Put(s, T.Home(T.R), 5, 4);
            MatchEngine.Apply(s, GameAction.Power(T.Home(T.R), new Tile(7, 4)));
            Assert.That(s.Team(Side.Home).Flux, Is.EqualTo(5 - Catalog.Blink.FluxCost));
            Assert.That(s.GetPlayer(T.Home(T.R)).PowerCooldown, Is.EqualTo(Catalog.Blink.Cooldown));
            Assert.That(MatchEngine.Validate(s, GameAction.Power(T.Home(T.R), new Tile(8, 4))), Does.Contain("cooling down"));
        }

        [Test]
        public void PowersNeedEnoughFlux()
        {
            var s = T.Empty(Catalog.Blinkers, Catalog.Gravions);
            Scenario.SetFlux(s, Side.Home, Catalog.Blink.FluxCost - 1);
            T.Put(s, T.Home(T.R), 5, 4);
            Assert.That(MatchEngine.Validate(s, GameAction.Power(T.Home(T.R), new Tile(7, 4))), Does.Contain("Flux"));
        }

        [Test]
        public void GravityWellRipsTheBallLooseFromRange()
        {
            var s = T.Empty(Catalog.Gravions, Catalog.Blinkers);
            Scenario.SetFlux(s, Side.Home, 10);
            T.Put(s, T.Home(T.P), 5, 4);
            T.PutWithBall(s, T.Away(T.S), 8, 4);
            var result = MatchEngine.Apply(s, GameAction.Power(T.Home(T.P)));
            Assert.That(s.BallIsLoose, Is.True);
            Assert.That(s.LooseBallTile, Is.EqualTo(new Tile(6, 4)));
            Assert.That(result.Events(EventType.BallPulled), Has.Count.EqualTo(1));
        }

        [Test]
        public void GravityWellFromTwoTilesLandsAtTheUsersFeet()
        {
            var s = T.Empty(Catalog.Gravions, Catalog.Blinkers);
            Scenario.SetFlux(s, Side.Home, 10);
            T.Put(s, T.Home(T.P), 5, 4);
            T.PutWithBall(s, T.Away(T.S), 7, 4);
            MatchEngine.Apply(s, GameAction.Power(T.Home(T.P)));
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Home(T.P))));
        }

        [Test]
        public void GravityWellStopsAtTheFirstPlayerInTheWay()
        {
            var s = T.Empty(Catalog.Gravions, Catalog.Blinkers);
            Scenario.SetFlux(s, Side.Home, 10);
            T.Put(s, T.Home(T.P), 5, 4);
            T.Put(s, T.Away(T.A), 7, 4);
            T.PutWithBall(s, T.Away(T.S), 8, 4);
            MatchEngine.Apply(s, GameAction.Power(T.Home(T.P)));
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Away(T.A))));
        }

        [Test]
        public void GravityWellHasARangeAndNeedsTheBallElsewhere()
        {
            var s = T.Empty(Catalog.Gravions, Catalog.Blinkers);
            Scenario.SetFlux(s, Side.Home, 10);
            T.Put(s, T.Home(T.P), 5, 4);
            T.PutWithBall(s, T.Away(T.S), 10, 4);
            Assert.That(MatchEngine.Validate(s, GameAction.Power(T.Home(T.P))), Does.Contain("tiles away"));
            T.PutWithBall(s, T.Home(T.Q), 6, 6);
            Assert.That(MatchEngine.Validate(s, GameAction.Power(T.Home(T.P))), Does.Contain("already has the ball"));
        }

        [Test]
        public void ShockwavePushesNeighboursAndKnocksTheBallLoose()
        {
            var s = T.Empty(Catalog.Gravions, Catalog.Blinkers);
            Scenario.SetFlux(s, Side.Home, 10);
            T.Put(s, T.Home(T.A), 6, 4);
            T.PutWithBall(s, T.Away(T.S), 7, 4);
            T.Put(s, T.Away(T.Q), 6, 5);
            var result = MatchEngine.Apply(s, GameAction.Power(T.Home(T.A)));
            Assert.That(s.GetPlayer(T.Away(T.S)).Pos, Is.EqualTo(new Tile(8, 4)));
            Assert.That(s.GetPlayer(T.Away(T.Q)).Pos, Is.EqualTo(new Tile(6, 6)));
            Assert.That(s.BallIsLoose, Is.True);
            Assert.That(s.LooseBallTile, Is.EqualTo(new Tile(7, 4)));
            Assert.That(result.Events(EventType.Pushed), Has.Count.EqualTo(2));
        }

        [Test]
        public void ShockwaveCannotPushOffThePitchOrMoveAnAnchoredCarrierOffTheBall()
        {
            var s = T.Empty(Catalog.Gravions, Catalog.Gravions);
            Scenario.SetFlux(s, Side.Home, 10);
            T.Put(s, T.Home(T.A), 6, 7);
            T.Put(s, T.Away(T.Q), 6, 8);         // touchline behind: can't move
            T.PutWithBall(s, T.Away(T.S), 7, 7); // Gravion: Anchored
            var result = MatchEngine.Apply(s, GameAction.Power(T.Home(T.A)));
            Assert.That(result.Events(EventType.PushBlocked), Has.Count.EqualTo(1));
            Assert.That(s.GetPlayer(T.Away(T.Q)).Pos, Is.EqualTo(new Tile(6, 8)));
            Assert.That(s.GetPlayer(T.Away(T.S)).Pos, Is.EqualTo(new Tile(8, 7)));
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Away(T.S))));
        }

        [Test]
        public void MeteorStrikeCannotBeBlocked()
        {
            var s = T.Empty(Catalog.Gravions, Catalog.Blinkers);
            Scenario.SetFlux(s, Side.Home, 10);
            T.PutWithBall(s, T.Home(T.S), 11, 4);
            T.Put(s, T.Away(T.A), 12, 4);
            T.Put(s, T.Away(T.K), 14, 4);
            Assert.That(MatchEngine.Preview(s, GameAction.Shoot(T.Home(T.S), 4)).Contests, Has.Count.EqualTo(2));
            var meteor = MatchEngine.Preview(s, GameAction.PowerAtGoal(T.Home(T.S), 4)).Contests;
            Assert.That(meteor, Has.Count.EqualTo(1));
            Assert.That(meteor[0].Kind, Is.EqualTo(ContestKind.Save));
        }

        [Test]
        public void BlinkTeleportsWithTheBallAndNoContests()
        {
            var s = T.Empty(Catalog.Blinkers, Catalog.Gravions);
            Scenario.SetFlux(s, Side.Home, 10);
            T.PutWithBall(s, T.Home(T.S), 5, 4);
            T.Put(s, T.Away(T.A), 6, 4);
            T.Put(s, T.Away(T.B), 7, 5);
            var result = MatchEngine.Apply(s, GameAction.Power(T.Home(T.S), new Tile(8, 4)));
            Assert.That(result.Contests(), Is.Empty);
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Home(T.S))));
            Assert.That(s.BallTile, Is.EqualTo(new Tile(8, 4)));
            Assert.That(MatchEngine.Validate(s, GameAction.Power(T.Home(T.R), new Tile(1, 1))), Does.Contain("not on the pitch"));
        }

        [Test]
        public void BlinkReachesThreeTiles()
        {
            var s = T.Empty(Catalog.Blinkers, Catalog.Gravions);
            Scenario.SetFlux(s, Side.Home, 10);
            T.Put(s, T.Home(T.S), 5, 4);
            Assert.That(MatchEngine.Validate(s, GameAction.Power(T.Home(T.S), new Tile(9, 4))), Does.Contain("1–3"));
        }

        [Test]
        public void PhasePassCannotBeIntercepted()
        {
            var s = T.Empty(Catalog.Blinkers, Catalog.Gravions);
            Scenario.SetFlux(s, Side.Home, 10);
            T.PutWithBall(s, T.Home(T.P), 5, 4);
            T.Put(s, T.Home(T.S), 9, 4);
            T.Put(s, T.Away(T.A), 7, 4);
            Assert.That(MatchEngine.Preview(s, GameAction.Pass(T.Home(T.P), new Tile(9, 4))).Contests, Has.Count.EqualTo(1));
            var result = T.Fail(s, GameAction.Power(T.Home(T.P), new Tile(9, 4)));
            Assert.That(result.Contests(), Is.Empty);
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Home(T.S))));
        }

        [Test]
        public void RiftTackleTeleportsNextToTheCarrierAndTacklesWithABonus()
        {
            var s = T.Empty(Catalog.Blinkers, Catalog.Gravions);
            Scenario.SetFlux(s, Side.Home, 10);
            T.Put(s, T.Home(T.A), 5, 4);
            T.PutWithBall(s, T.Away(T.S), 8, 4);
            var rift = GameAction.Power(T.Home(T.A));
            var c = MatchEngine.Preview(s, rift).Contests;
            int tackle = s.GetPlayer(T.Home(T.A)).Stats.Tackle;
            int pace = s.GetPlayer(T.Away(T.S)).Stats.Pace;
            Assert.That(c, Has.Count.EqualTo(1));
            Assert.That(c[0].Kind, Is.EqualTo(ContestKind.Tackle));
            Assert.That(c[0].Chance, Is.EqualTo(Rules.Contests.Chance(s.Config, tackle, pace, s.Config.RiftTackleModifier)));

            var result = T.Succeed(s, rift);
            var tackler = s.GetPlayer(T.Home(T.A));
            Assert.That(tackler.Pos.DistanceTo(new Tile(8, 4)), Is.EqualTo(1));
            Assert.That(s.Carrier, Is.SameAs(tackler));
            Assert.That(result.Events(EventType.Teleported), Has.Count.EqualTo(1));
        }

        [Test]
        public void RiftTackleReachesThreeTiles()
        {
            var s = T.Empty(Catalog.Blinkers, Catalog.Gravions);
            Scenario.SetFlux(s, Side.Home, 10);
            T.Put(s, T.Home(T.A), 4, 4);
            T.PutWithBall(s, T.Away(T.S), 8, 4);
            Assert.That(MatchEngine.Validate(s, GameAction.Power(T.Home(T.A))), Does.Contain("more than 3"));
        }

        [Test]
        public void ACooldownRunsOutAfterItsTurns()
        {
            var s = T.Empty(Catalog.Blinkers, Catalog.Gravions);
            Scenario.SetFlux(s, Side.Home, 10);
            T.Put(s, T.Home(T.R), 5, 4);
            MatchEngine.Apply(s, GameAction.Power(T.Home(T.R), new Tile(7, 4)));
            for (int i = 0; i < 2 * Catalog.Blink.Cooldown; i++) MatchEngine.Apply(s, GameAction.EndTurn());
            Assert.That(s.ActiveSide, Is.EqualTo(Side.Home));
            Assert.That(MatchEngine.Validate(s, GameAction.Power(T.Home(T.R), new Tile(8, 4))), Is.Null);
        }
    }

    public class UltimateTests
    {
        [Test]
        public void UltimatesNeedFullFlux()
        {
            var s = T.NewMatch(Catalog.Gravions, Catalog.Blinkers);
            Scenario.SetFlux(s, Side.Home, s.Config.MaxFlux - 1);
            Assert.That(MatchEngine.Validate(s, GameAction.Ultimate()), Does.Contain("full Flux"));
        }

        [Test]
        public void AnUltimateIsFreeAndOncePerMatch()
        {
            var s = T.NewMatch(Catalog.Gravions, Catalog.Blinkers);
            Scenario.SetFlux(s, Side.Home, s.Config.MaxFlux);
            var result = MatchEngine.Apply(s, GameAction.Ultimate());
            Assert.That(result.Events(EventType.UltimateUsed), Has.Count.EqualTo(1));
            Assert.That(s.ActionsLeft, Is.EqualTo(3));
            Assert.That(s.Team(Side.Home).Flux, Is.EqualTo(0));
            Scenario.SetFlux(s, Side.Home, s.Config.MaxFlux);
            Assert.That(MatchEngine.Validate(s, GameAction.Ultimate()), Does.Contain("already been used"));
        }

        [Test]
        public void CrushHalvesPaceUntilTheEndOfTheOpponentsNextTurn()
        {
            var s = T.NewMatch(Catalog.Gravions, Catalog.Blinkers);
            var runner = s.GetPlayer(T.Away(T.R)); // Pace 9, moves 4
            Scenario.SetFlux(s, Side.Home, s.Config.MaxFlux);
            MatchEngine.Apply(s, GameAction.Ultimate());
            Assert.That(s.EffectivePace(runner), Is.EqualTo(4));
            Assert.That(s.MoveRange(runner), Is.EqualTo(2));
            MatchEngine.Apply(s, GameAction.EndTurn());
            Assert.That(s.ActiveSide, Is.EqualTo(Side.Away));
            Assert.That(s.MoveRange(runner), Is.EqualTo(2), "still halved on their turn");
            MatchEngine.Apply(s, GameAction.EndTurn());
            Assert.That(s.MoveRange(runner), Is.EqualTo(4), "back to normal afterwards");
        }

        [Test]
        public void FoldSpaceSwapsTwoPlayersAndTheBallGoesWithItsCarrier()
        {
            var s = T.NewMatch(Catalog.Blinkers, Catalog.Gravions);
            Scenario.SetFlux(s, Side.Home, s.Config.MaxFlux);
            var striker = s.GetPlayer(T.Home(T.S));
            var anchor = s.GetPlayer(T.Home(T.A));
            var strikerWas = striker.Pos;
            var anchorWas = anchor.Pos;
            MatchEngine.Apply(s, GameAction.Ultimate(striker.Id, anchor.Id));
            Assert.That(striker.Pos, Is.EqualTo(anchorWas));
            Assert.That(anchor.Pos, Is.EqualTo(strikerWas));
            Assert.That(s.BallTile, Is.EqualTo(anchorWas));
        }

        [Test]
        public void FoldSpaceCannotTakeTheKeeperOutOfTheBox()
        {
            var s = T.NewMatch(Catalog.Blinkers, Catalog.Gravions);
            Scenario.SetFlux(s, Side.Home, s.Config.MaxFlux);
            Assert.That(MatchEngine.Validate(s, GameAction.Ultimate(T.Home(T.K), T.Home(T.S))), Does.Contain("penalty area"));
            Assert.That(MatchEngine.Validate(s, GameAction.Ultimate(T.Home(T.S), T.Home(T.S))), Does.Contain("different"));
            Assert.That(MatchEngine.Validate(s, GameAction.Ultimate(T.Home(T.S), T.Away(T.S))), Does.Contain("yours"));
        }
    }
}
