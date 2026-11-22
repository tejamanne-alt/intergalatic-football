using System.Collections.Generic;
using System.Linq;

namespace IntergalacticFootball.Rules.Tests
{
    /// <summary>Helpers for building positions. Player ids: Home 0–6, Away 7–13, in slot order K A B P Q R S.</summary>
    static class T
    {
        public const int K = 0, A = 1, B = 2, P = 3, Q = 4, R = 5, S = 6;

        public static int Home(int slot) => slot;

        public static int Away(int slot) => Squad.Size + slot;

        /// <summary>A plain species with base stats, no passive and no powers, for testing the core rules.</summary>
        public static readonly SpeciesDef Testers = new SpeciesDef(
            "testers", "Testers", "Test", Passive.None, "None", "",
            new[] { Role.Keeper, Role.Anchor, Role.Playmaker, Role.Runner, Role.Striker }.Select(Catalog.BaseStats).ToArray(),
            new[] { PowerId.GravityWell, PowerId.Shockwave, PowerId.MeteorStrike, PowerId.Blink, PowerId.PhasePass, PowerId.RiftTackle },
            Enumerable.Repeat(PowerId.None, Squad.Size).ToArray(),
            UltimateId.None);

        public static MatchState NewMatch(SpeciesDef home = null, SpeciesDef away = null, ulong seed = 1, Formation formation = null)
        {
            var setup = new MatchSetup(new TeamSetup(home ?? Testers, formation), new TeamSetup(away ?? Testers, formation), seed);
            return MatchState.Create(setup);
        }

        /// <summary>Home to act with 3 actions, nobody on the pitch, ball parked in a far corner.</summary>
        public static MatchState Empty(SpeciesDef home = null, SpeciesDef away = null)
        {
            var s = NewMatch(home, away);
            Scenario.ClearPitch(s);
            Scenario.PlaceBall(s, new Tile(0, 0));
            return s;
        }

        public static void Put(MatchState s, int id, int x, int y) => Scenario.Place(s, id, new Tile(x, y));

        public static void PutWithBall(MatchState s, int id, int x, int y)
        {
            Scenario.Place(s, id, new Tile(x, y));
            Scenario.GiveBall(s, id);
        }

        public static Tile[] Path(params int[] xy)
        {
            var tiles = new Tile[xy.Length / 2];
            for (int i = 0; i < tiles.Length; i++) tiles[i] = new Tile(xy[2 * i], xy[2 * i + 1]);
            return tiles;
        }

        public static ActionResult Succeed(MatchState s, GameAction a) => MatchEngine.Apply(s, a, ScriptedRolls.AlwaysSucceed());

        public static ActionResult Fail(MatchState s, GameAction a) => MatchEngine.Apply(s, a, ScriptedRolls.AlwaysFail());

        public static ActionResult Roll(MatchState s, GameAction a, params bool[] outcomes) => MatchEngine.Apply(s, a, new ScriptedRolls(true, outcomes));

        public static List<MatchEvent> Events(this ActionResult r, EventType type) => r.Events.Where(e => e.Type == type).ToList();

        public static List<MatchEvent> Contests(this ActionResult r) => r.Events(EventType.Contest);
    }
}
