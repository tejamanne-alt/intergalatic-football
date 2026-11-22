using System;
using System.Linq;
using NUnit.Framework;

namespace IntergalacticFootball.Rules.Tests
{
    public class GeometryTests
    {
        [Test]
        public void DistanceIsChebyshev()
        {
            Assert.That(Tile.Distance(new Tile(0, 0), new Tile(3, 1)), Is.EqualTo(3));
            Assert.That(Tile.Distance(new Tile(2, 2), new Tile(3, 3)), Is.EqualTo(1));
        }

        [Test]
        public void LineExcludesStartAndIncludesEnd()
        {
            Assert.That(Pitch.Line(new Tile(0, 0), new Tile(3, 0)), Is.EqualTo(T.Path(1, 0, 2, 0, 3, 0)));
            Assert.That(Pitch.Line(new Tile(0, 0), new Tile(2, 2)), Is.EqualTo(T.Path(1, 1, 2, 2)));
            Assert.That(Pitch.Line(new Tile(0, 0), new Tile(3, 1)), Is.EqualTo(T.Path(1, 0, 2, 1, 3, 1)));
        }

        [Test]
        public void PenaltyAreasAreThreeDeepOnRowsOneToSeven()
        {
            Assert.That(Pitch.InPenaltyArea(Side.Home, new Tile(2, 1)), Is.True);
            Assert.That(Pitch.InPenaltyArea(Side.Home, new Tile(3, 4)), Is.False);
            Assert.That(Pitch.InPenaltyArea(Side.Home, new Tile(0, 0)), Is.False);
            Assert.That(Pitch.InPenaltyArea(Side.Away, new Tile(12, 7)), Is.True);
            Assert.That(Pitch.InPenaltyArea(Side.Away, new Tile(2, 4)), Is.False);
        }

        [Test]
        public void ProgressRunsTowardTheOpponentsGoal()
        {
            Assert.That(Pitch.Progress(Side.Home, new Tile(10, 4)), Is.EqualTo(10));
            Assert.That(Pitch.Progress(Side.Away, new Tile(10, 4)), Is.EqualTo(4));
        }

        [Test]
        public void TileParsesXCommaY()
        {
            Assert.That(Tile.TryParse("9, 4", out var t), Is.True);
            Assert.That(t, Is.EqualTo(new Tile(9, 4)));
            Assert.That(Tile.TryParse("nine", out _), Is.False);
        }
    }

    public class SetupTests
    {
        [Test]
        public void HomeStrikerKicksOffFromTheCentre()
        {
            var s = T.NewMatch(Catalog.Gravions, Catalog.Blinkers);
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Home(T.S))));
            Assert.That(s.BallTile, Is.EqualTo(Pitch.Centre));
            Assert.That(s.ActiveSide, Is.EqualTo(Side.Home));
            Assert.That(s.ActionsLeft, Is.EqualTo(3));
            Assert.That(s.TurnIndex, Is.EqualTo(0));
        }

        [Test]
        public void EveryFormationKicksOffLegally([ValueSource(nameof(Formations))] Formation home, [ValueSource(nameof(Formations))] Formation away)
        {
            var setup = new MatchSetup(new TeamSetup(T.Testers, home), new TeamSetup(T.Testers, away));
            var s = MatchState.Create(setup);
            Assert.That(s.Players.Select(p => p.Pos).Distinct().Count(), Is.EqualTo(14), "players share a tile");
            foreach (var p in s.PlayersOf(Side.Home)) Assert.That(p.Pos.X, Is.LessThanOrEqualTo(7));
            foreach (var p in s.PlayersOf(Side.Away))
            {
                Assert.That(p.Pos.X, Is.GreaterThanOrEqualTo(9));
                Assert.That(p.Pos.DistanceTo(Pitch.Centre), Is.GreaterThan(1), "defender touching the kick-off");
            }
            Assert.That(Pitch.InPenaltyArea(Side.Home, s.Keeper(Side.Home).Pos), Is.True);
            Assert.That(Pitch.InPenaltyArea(Side.Away, s.Keeper(Side.Away).Pos), Is.True);
        }

        static Formation[] Formations => Formation.All.ToArray();

        [Test]
        public void PlayersGetSpeciesStatsAndLoadout()
        {
            var s = T.NewMatch(Catalog.Gravions, Catalog.Blinkers);
            var striker = s.GetPlayer(T.Home(T.S));
            Assert.That(striker.Stats.Shot, Is.EqualTo(Catalog.Gravions.StatsFor(Role.Striker).Shot));
            Assert.That(striker.Power, Is.EqualTo(PowerId.MeteorStrike));
            Assert.That(s.GetPlayer(T.Away(T.R)).Power, Is.EqualTo(PowerId.Blink));
            Assert.That(striker.Label, Is.EqualTo("S"));
            Assert.That(s.GetPlayer(T.Away(T.S)).Label, Is.EqualTo("s"));
        }

        [Test]
        public void LoadoutMustComeFromTheSpeciesPool()
        {
            var loadout = Enumerable.Repeat(PowerId.Blink, Squad.Size).ToArray();
            Assert.Throws<ArgumentException>(() => new TeamSetup(Catalog.Gravions, null, loadout));
        }

        [Test]
        public void MoveRangeFollowsPace()
        {
            var cfg = new RulesConfig();
            Assert.That(cfg.MoveRange(3), Is.EqualTo(2));
            Assert.That(cfg.MoveRange(6), Is.EqualTo(3));
            Assert.That(cfg.MoveRange(9), Is.EqualTo(4));
            var s = T.NewMatch(Catalog.Gravions, Catalog.Blinkers);
            Assert.That(s.MoveRange(s.GetPlayer(T.Away(T.R))), Is.EqualTo(4), "Blinker runner, Pace 9");
            Assert.That(s.MoveRange(s.GetPlayer(T.Home(T.A))), Is.EqualTo(2), "Gravion anchor, Pace 4");
        }
    }

    public class TurnTests
    {
        [Test]
        public void ThreeActionsThenTheTurnPasses()
        {
            var s = T.NewMatch();
            var q = s.GetPlayer(T.Home(T.Q));
            T.Succeed(s, GameAction.Move(q.Id, T.Path(5, 1)));
            T.Succeed(s, GameAction.Move(q.Id, T.Path(6, 1)));
            Assert.That(s.ActionsLeft, Is.EqualTo(1));
            Assert.That(s.ActiveSide, Is.EqualTo(Side.Home));
            var result = T.Succeed(s, GameAction.Move(q.Id, T.Path(5, 1)));
            Assert.That(result.Events(EventType.TurnEnded), Has.Count.EqualTo(1));
            Assert.That(s.ActiveSide, Is.EqualTo(Side.Away));
            Assert.That(s.ActionsLeft, Is.EqualTo(3));
            Assert.That(s.TurnIndex, Is.EqualTo(1));
        }

        [Test]
        public void EndTurnPassesEarly()
        {
            var s = T.NewMatch();
            MatchEngine.Apply(s, GameAction.EndTurn());
            Assert.That(s.ActiveSide, Is.EqualTo(Side.Away));
            Assert.That(s.ActionsLeft, Is.EqualTo(3));
        }

        [Test]
        public void TheOtherSideCannotAct()
        {
            var s = T.NewMatch();
            var error = MatchEngine.Validate(s, GameAction.Move(T.Away(T.Q), T.Path(9, 1)));
            Assert.That(error, Does.Contain("not on the team to act"));
        }

        [Test]
        public void EachSidePlaysTenTurnsPerHalf()
        {
            var s = T.NewMatch();
            Assert.That(Enumerable.Range(0, 20).Count(t => s.SideForTurn(t) == Side.Home), Is.EqualTo(10));
            Assert.That(Enumerable.Range(20, 20).Count(t => s.SideForTurn(t) == Side.Away), Is.EqualTo(10));
            Assert.That(s.SideForTurn(20), Is.EqualTo(Side.Away), "away kicks off the second half");
        }

        [Test]
        public void HalfTimeResetsAndAwayKicksOff()
        {
            var s = T.NewMatch();
            for (int i = 0; i < 19; i++) MatchEngine.Apply(s, GameAction.EndTurn());
            var result = MatchEngine.Apply(s, GameAction.EndTurn());
            Assert.That(result.Events(EventType.HalfTime), Has.Count.EqualTo(1));
            Assert.That(s.Half, Is.EqualTo(2));
            Assert.That(s.ActiveSide, Is.EqualTo(Side.Away));
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Away(T.S))));
            Assert.That(s.BallTile, Is.EqualTo(Pitch.Centre));
        }

        [Test]
        public void FullTimeAfterFortyTeamTurns()
        {
            var s = T.NewMatch();
            for (int i = 0; i < 40; i++) MatchEngine.Apply(s, GameAction.EndTurn());
            Assert.That(s.IsOver, Is.True);
            Assert.That(MatchEngine.Validate(s, GameAction.EndTurn()), Does.Contain("over"));
            Assert.Throws<InvalidOperationException>(() => MatchEngine.Apply(s, GameAction.EndTurn()));
        }

        [Test]
        public void CooldownTicksAtTheStartOfTheOwnersTurns()
        {
            var s = T.NewMatch(Catalog.Gravions, Catalog.Blinkers);
            var p = s.GetPlayer(T.Home(T.P));
            Scenario.SetCooldown(s, p.Id, 2);
            MatchEngine.Apply(s, GameAction.EndTurn()); // away's turn
            Assert.That(p.PowerCooldown, Is.EqualTo(2));
            MatchEngine.Apply(s, GameAction.EndTurn()); // home's turn starts
            Assert.That(p.PowerCooldown, Is.EqualTo(1));
            MatchEngine.Apply(s, GameAction.EndTurn());
            MatchEngine.Apply(s, GameAction.EndTurn());
            Assert.That(p.PowerCooldown, Is.EqualTo(0));
        }
    }
}
