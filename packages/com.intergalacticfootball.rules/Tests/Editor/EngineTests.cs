using System.Collections.Generic;
using System.Linq;
using IntergalacticFootball.Rules.Ai;
using NUnit.Framework;

namespace IntergalacticFootball.Rules.Tests
{
    public class DeterminismTests
    {
        [Test]
        public void Pcg32MatchesTheReferenceImplementation()
        {
            // pcg32-demo from pcg-c-basic, seeded with (42, 54). If this changes, saved replays and server checks break.
            var rng = new Pcg32(42, 54);
            var values = Enumerable.Range(0, 6).Select(_ => rng.NextUInt()).ToArray();
            Assert.That(values, Is.EqualTo(new uint[] { 0xa15c02b7, 0x7b47f409, 0xba1d3330, 0x83d2f293, 0xbfa4784b, 0xcbed606e }));
        }

        [Test]
        public void RollsAreUniformEnough()
        {
            var rng = new Pcg32(7);
            var counts = new int[10];
            for (int i = 0; i < 100000; i++) counts[rng.NextInt(100) / 10]++;
            foreach (var c in counts) Assert.That(c, Is.InRange(9500, 10500));
        }

        [Test]
        public void TheSameSeedAndActionsReplayTheSameMatch()
        {
            var setup = new MatchSetup(new TeamSetup(Catalog.Gravions), new TeamSetup(Catalog.Blinkers), seed: 99);
            var live = MatchState.Create(setup);
            var actions = new List<GameAction>();
            var home = new AiPlayer(AiDifficulty.Easy, seed: 1);
            var away = new AiPlayer(AiDifficulty.Easy, seed: 2);
            var liveEvents = new List<string>();
            while (!live.IsOver)
            {
                var a = (live.ActiveSide == Side.Home ? home : away).ChooseAction(live);
                actions.Add(a);
                liveEvents.AddRange(MatchEngine.Apply(live, a).Events.Select(e => EventText.Describe(live, e)));
            }

            var replay = MatchState.Create(setup);
            var replayEvents = new List<string>();
            foreach (var a in actions) replayEvents.AddRange(MatchEngine.Apply(replay, a).Events.Select(e => EventText.Describe(replay, e)));

            Assert.That(replayEvents, Is.EqualTo(liveEvents));
            Assert.That(replay.Team(Side.Home).Score, Is.EqualTo(live.Team(Side.Home).Score));
            Assert.That(replay.Team(Side.Away).Score, Is.EqualTo(live.Team(Side.Away).Score));
            Assert.That(replay.Players.Select(p => p.Pos), Is.EqualTo(live.Players.Select(p => p.Pos)));
        }
    }

    public class AiTests
    {
        [Test]
        public void GeneratedActionsAreAllLegal()
        {
            var s = T.NewMatch(Catalog.Gravions, Catalog.Blinkers, seed: 5);
            var ai = new AiPlayer(AiDifficulty.Easy, seed: 3);
            for (int i = 0; i < 60 && !s.IsOver; i++)
            {
                Scenario.SetFlux(s, s.ActiveSide, s.Config.MaxFlux); // exercise powers and ultimates too
                foreach (var a in ActionGenerator.ForActiveSide(s, compact: false))
                    Assert.That(MatchEngine.Validate(s, a), Is.Null, a.ToString());
                MatchEngine.Apply(s, ai.ChooseAction(s));
            }
        }

        [Test]
        public void TheAiPlaysAWholeMatchLegally([Values(AiDifficulty.Easy, AiDifficulty.Medium)] AiDifficulty level)
        {
            var s = T.NewMatch(Catalog.Blinkers, Catalog.Gravions, seed: 17);
            var ai = new AiPlayer(level, seed: 4);
            int decisions = 0;
            while (!s.IsOver)
            {
                var a = ai.ChooseAction(s);
                Assert.That(MatchEngine.Validate(s, a), Is.Null, a.ToString());
                MatchEngine.Apply(s, a);
                Assert.That(++decisions, Is.LessThan(500), "match never ended");
            }
            Assert.That(s.TurnIndex, Is.EqualTo(s.Config.TotalTurns));
        }

        [Test]
        public void TheAiShootsAtAnOpenGoal()
        {
            var s = T.Empty(Catalog.Gravions, Catalog.Blinkers);
            T.PutWithBall(s, T.Home(T.S), 12, 4);
            Scenario.SetTurn(s, 0, actionsLeft: 1);
            var a = new AiPlayer(AiDifficulty.Medium).ChooseAction(s);
            Assert.That(a.Type == ActionType.Shoot || a.Type == ActionType.Power, Is.True, a.ToString());
        }

        [Test]
        public void TheAiTacklesAStrikerAboutToScore()
        {
            var s = T.Empty(Catalog.Blinkers, Catalog.Gravions);
            T.Put(s, T.Home(T.K), 0, 4);
            T.Put(s, T.Home(T.A), 3, 3);
            T.PutWithBall(s, T.Away(T.S), 2, 4);
            Scenario.SetTurn(s, 0, actionsLeft: 1);
            var a = new AiPlayer(AiDifficulty.Medium).ChooseAction(s);
            Assert.That(a.Type == ActionType.Tackle || a.Type == ActionType.Power, Is.True, a.ToString());
        }

        [Test]
        public void OutcomeProbabilitiesSumToOne()
        {
            var s = T.Empty();
            T.PutWithBall(s, T.Home(T.P), 3, 4);
            T.Put(s, T.Home(T.Q), 9, 4);
            T.Put(s, T.Away(T.A), 5, 3);
            T.Put(s, T.Away(T.B), 10, 5);
            var outcomes = Outcomes.Enumerate(s, GameAction.Pass(T.Home(T.P), new Tile(9, 4)));
            Assert.That(outcomes, Has.Count.EqualTo(3));
            Assert.That(outcomes.Sum(o => o.Probability), Is.EqualTo(1.0).Within(1e-9));
            Assert.That(outcomes.Count(o => o.State.Carrier == o.State.GetPlayer(T.Home(T.Q))), Is.EqualTo(1));
            Assert.That(s.Carrier, Is.SameAs(s.GetPlayer(T.Home(T.P))), "enumeration must not touch the real state");
        }
    }

    public class EngineFreeTests
    {
        [Test]
        public void TheReadmeExampleRuns()
        {
            var setup = new MatchSetup(
                new TeamSetup(Catalog.Gravions, Formation.TwoThreeOne),
                new TeamSetup(Catalog.Blinkers, Formation.ThreeTwoOne),
                seed: 1234);
            var match = MatchState.Create(setup);

            var striker = match.GetPlayer(6);
            var plans = Pathing.Reachable(match, striker);
            var pass = GameAction.Pass(striker.Id, match.GetPlayer(3).Pos);
            var preview = MatchEngine.Preview(match, pass);
            var result = MatchEngine.Apply(match, pass);

            var ai = new AiPlayer(AiDifficulty.Medium);
            MatchEngine.Apply(match, GameAction.EndTurn());
            while (!match.IsOver && match.ActiveSide == Side.Away)
                MatchEngine.Apply(match, ai.ChooseAction(match));

            Assert.That(plans, Is.Not.Empty);
            Assert.That(preview.IsLegal, Is.True);
            Assert.That(result.Events, Is.Not.Empty);
            Assert.That(match.ActiveSide, Is.EqualTo(Side.Home));
        }

        [Test]
        public void TheRulesAssemblyHasNoUnityReferences()
        {
            var references = typeof(MatchState).Assembly.GetReferencedAssemblies().Select(a => a.Name).ToList();
            Assert.That(references.Where(n => n.StartsWith("UnityEngine") || n.StartsWith("UnityEditor")), Is.Empty);
        }
    }
}
