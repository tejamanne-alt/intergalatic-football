using System.Collections.Generic;

namespace IntergalacticFootball.Rules
{
    /// <summary>
    /// Lists legal actions: for the AI, and for the UI to highlight what a selected player can do.
    /// </summary>
    public static class ActionGenerator
    {
        /// <summary>
        /// Legal actions for the side to act. With <paramref name="compact"/>, passes into space are limited to
        /// tiles just ahead of a teammate (through balls), which keeps the AI's search small; otherwise every
        /// tile in range is included.
        /// </summary>
        public static List<GameAction> ForActiveSide(MatchState s, bool compact = true)
        {
            var actions = new List<GameAction>(256);
            if (s.IsOver) return actions;
            actions.Add(GameAction.EndTurn());
            var side = s.ActiveSide;
            AddUltimates(s, side, actions);
            if (s.ActionsLeft <= 0) return actions;

            foreach (var p in s.PlayersOf(side))
            {
                if (!s.IsOnPitch(p)) continue;
                foreach (var plan in Pathing.Reachable(s, p)) actions.Add(GameAction.Move(p.Id, plan.Path));
                if (s.BallCarrierId == p.Id)
                {
                    foreach (var t in PassTargets(s, p, compact)) actions.Add(GameAction.Pass(p.Id, t));
                    foreach (var row in ShotRows(s, p)) actions.Add(GameAction.Shoot(p.Id, row));
                }
                if (CanTackle(s, p)) actions.Add(GameAction.Tackle(p.Id));
                AddPowers(s, p, compact, actions);
            }
            return actions;
        }

        public static bool CanTackle(MatchState s, Player p)
        {
            var carrier = s.Carrier;
            return carrier != null && carrier.Side != p.Side && s.IsOnPitch(p) && p.Pos.DistanceTo(carrier.Pos) == 1;
        }

        public static List<int> ShotRows(MatchState s, Player shooter)
        {
            var rows = new List<int>(3);
            for (int row = Pitch.GoalTop; row <= Pitch.GoalBottom; row++)
                if (Contests.ShotInRange(s, shooter, row)) rows.Add(row);
            return rows;
        }

        public static List<Tile> PassTargets(MatchState s, Player passer, bool compact)
        {
            var result = new List<Tile>();
            int range = s.PassRange(passer);
            if (!compact)
            {
                for (int y = 0; y < Pitch.Height; y++)
                for (int x = 0; x < Pitch.Width; x++)
                {
                    var t = new Tile(x, y);
                    int d = passer.Pos.DistanceTo(t);
                    if (d < 1 || d > range) continue;
                    var occupant = s.PlayerAt(t);
                    if (occupant != null && occupant.Side != passer.Side) continue;
                    result.Add(t);
                }
                return result;
            }

            var seen = new HashSet<Tile>();
            int forward = passer.Side == Side.Home ? 1 : -1;
            foreach (var mate in s.PlayersOf(passer.Side))
            {
                if (mate == passer || !s.IsOnPitch(mate)) continue;
                if (passer.Pos.DistanceTo(mate.Pos) <= range && seen.Add(mate.Pos)) result.Add(mate.Pos);
                if (mate.IsKeeper) continue;
                for (int dy = -1; dy <= 1; dy++)
                {
                    var t = mate.Pos.Offset(forward, dy);
                    if (!Pitch.InBounds(t) || s.PlayerAt(t) != null) continue;
                    int d = passer.Pos.DistanceTo(t);
                    if (d >= 1 && d <= range && seen.Add(t)) result.Add(t);
                }
            }
            return result;
        }

        static void TryAdd(MatchState s, GameAction a, List<GameAction> into)
        {
            if (Validation.Check(s, a) == null) into.Add(a);
        }

        static void AddPowers(MatchState s, Player p, bool compact, List<GameAction> into)
        {
            var def = Catalog.Power(p.Power);
            if (def == null || p.PowerCooldown > 0 || s.Team(p.Side).Flux < def.FluxCost) return;
            switch (def.Id)
            {
                case PowerId.GravityWell:
                case PowerId.Shockwave:
                case PowerId.RiftTackle:
                    TryAdd(s, GameAction.Power(p.Id), into);
                    break;
                case PowerId.MeteorStrike:
                    for (int row = Pitch.GoalTop; row <= Pitch.GoalBottom; row++) TryAdd(s, GameAction.PowerAtGoal(p.Id, row), into);
                    break;
                case PowerId.Blink:
                    for (int dy = -def.Range; dy <= def.Range; dy++)
                    for (int dx = -def.Range; dx <= def.Range; dx++)
                        if (dx != 0 || dy != 0) TryAdd(s, GameAction.Power(p.Id, p.Pos.Offset(dx, dy)), into);
                    break;
                case PowerId.PhasePass:
                    if (s.BallCarrierId != p.Id) break;
                    foreach (var t in PassTargets(s, p, compact)) TryAdd(s, GameAction.Power(p.Id, t), into);
                    break;
            }
        }

        static void AddUltimates(MatchState s, Side side, List<GameAction> into)
        {
            if (!s.UltimateReady(side)) return;
            var def = Catalog.Ultimate(s.Team(side).Species.Ultimate);
            if (def == null) return;
            if (!def.NeedsTwoPlayers)
            {
                TryAdd(s, GameAction.Ultimate(), into);
                return;
            }
            var squad = s.PlayersOf(side);
            for (int i = 0; i < squad.Count; i++)
            for (int j = i + 1; j < squad.Count; j++)
                TryAdd(s, GameAction.Ultimate(squad[i].Id, squad[j].Id), into);
        }
    }
}
