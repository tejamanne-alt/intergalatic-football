using System.Collections.Generic;

namespace IntergalacticFootball.Rules
{
    /// <summary>Legality checks. Each returns null when legal, otherwise a message a player can read.</summary>
    internal static class Validation
    {
        public static string Check(MatchState s, GameAction a)
        {
            if (a == null) return "No action given.";
            if (s.IsOver) return "The match is over.";
            if (a.Type == ActionType.EndTurn) return null;
            if (a.Type == ActionType.Ultimate) return CheckUltimate(s, a);
            if (s.ActionsLeft <= 0) return "No actions left this turn.";
            if (a.PlayerId < 0 || a.PlayerId >= s.Players.Count) return "Unknown player.";

            var p = s.GetPlayer(a.PlayerId);
            if (p.Side != s.ActiveSide) return $"{p.Label} is not on the team to act.";
            if (!s.IsOnPitch(p)) return $"{p.Label} is not on the pitch.";

            switch (a.Type)
            {
                case ActionType.Move: return CheckMove(s, p, a.Path);
                case ActionType.Pass: return a.HasTarget ? CheckPass(s, p, a.Target) : "A pass needs a target.";
                case ActionType.Shoot: return CheckShot(s, p, a.GoalRow);
                case ActionType.Tackle: return CheckTackle(s, p);
                case ActionType.Power: return CheckPower(s, p, a);
                default: return "Unknown action.";
            }
        }

        static string CheckMove(MatchState s, Player p, IReadOnlyList<Tile> path)
        {
            if (path == null || path.Count == 0) return "A move needs a path.";
            int range = s.MoveRange(p);
            if (path.Count > range) return $"{p.Label} can move at most {range} tiles.";
            bool carrying = s.BallCarrierId == p.Id;
            int handled = 0;
            int ignores = carrying && s.Team(p.Side).Species.Passive == Passive.Phasing ? 1 : 0;
            var previous = p.Pos;
            for (int i = 0; i < path.Count; i++)
            {
                var t = path[i];
                if (!Pitch.InBounds(t)) return $"{t} is off the pitch.";
                if (previous.DistanceTo(t) != 1) return "Each step must be to a neighbouring tile.";
                if (p.IsKeeper && !Pitch.InPenaltyArea(p.Side, t)) return "Keepers must stay in their penalty area.";
                var occupant = s.PlayerAt(t);
                if (occupant != null) return $"{t} is taken by {occupant.Label}.";
                bool last = i == path.Count - 1;
                if (!carrying && s.BallIsLoose && s.LooseBallTile == t && !last) return "Picking up the ball ends the move.";
                if (carrying)
                {
                    var stopper = ZoneStopper(s, p, t, ref handled, ref ignores);
                    if (stopper != null && !last) return $"Stepping into {stopper.Label}'s zone ends the move.";
                }
                previous = t;
            }
            return null;
        }

        /// <summary>
        /// The first enemy who would contest a carrier stepping onto <paramref name="t"/>, or null. Updates which
        /// enemies have been met this move and how many zones the carrier may still ignore (Phasing).
        /// </summary>
        internal static Player ZoneStopper(MatchState s, Player carrier, Tile t, ref int handled, ref int ignores)
        {
            Player first = null;
            foreach (var e in s.PlayersOf(carrier.Side.Opponent()))
            {
                int bit = 1 << e.Id;
                if ((handled & bit) != 0 || !Contests.InZone(e, t)) continue;
                handled |= bit;
                if (ignores > 0)
                {
                    ignores--;
                    continue;
                }
                if (first == null) first = e;
            }
            return first;
        }

        internal static string CheckPass(MatchState s, Player p, Tile target)
        {
            if (s.BallCarrierId != p.Id) return $"{p.Label} doesn't have the ball.";
            if (!Pitch.InBounds(target)) return $"{target} is off the pitch.";
            if (target == p.Pos) return "Pick a target other than the passer.";
            int range = s.PassRange(p);
            if (p.Pos.DistanceTo(target) > range) return $"Too far: {p.Label}'s pass range is {range} tiles.";
            var occupant = s.PlayerAt(target);
            if (occupant != null && occupant.Side != p.Side) return $"{occupant.Label} is standing there.";
            return null;
        }

        internal static string CheckShot(MatchState s, Player p, int row)
        {
            if (s.BallCarrierId != p.Id) return $"{p.Label} doesn't have the ball.";
            if (!Pitch.IsGoalRow(row)) return $"Aim at goal row {Pitch.GoalTop}–{Pitch.GoalBottom}.";
            int distance = p.Pos.DistanceTo(Pitch.GoalTile(p.Side.Opponent(), row));
            int range = s.ShotRange(p);
            if (distance > range) return $"Out of range: {distance} tiles to goal, {p.Label}'s shot range is {range}.";
            return null;
        }

        static string CheckTackle(MatchState s, Player p)
        {
            var carrier = s.Carrier;
            if (carrier == null || carrier.Side == p.Side) return "No opponent has the ball.";
            if (p.Pos.DistanceTo(carrier.Pos) != 1) return $"{p.Label} must be next to {carrier.Label} to tackle.";
            return null;
        }

        static string CheckPower(MatchState s, Player p, GameAction a)
        {
            var def = Catalog.Power(p.Power);
            if (def == null) return $"{p.Label} has no power.";
            if (p.PowerCooldown > 0) return $"{def.Name} is cooling down ({p.PowerCooldown} more turn{(p.PowerCooldown == 1 ? "" : "s")}).";
            var team = s.Team(p.Side);
            if (team.Flux < def.FluxCost) return $"{def.Name} costs {def.FluxCost} Flux; the team has {team.Flux}.";

            switch (def.Id)
            {
                case PowerId.GravityWell:
                {
                    var holder = s.Carrier;
                    if (holder != null && holder.Side == p.Side) return "Your team already has the ball.";
                    int d = p.Pos.DistanceTo(s.BallTile);
                    if (d > def.Range) return $"The ball is more than {def.Range} tiles away.";
                    return null;
                }
                case PowerId.Shockwave:
                    foreach (var e in s.PlayersOf(p.Side.Opponent()))
                        if (s.IsOnPitch(e) && e.Pos.DistanceTo(p.Pos) == 1) return null;
                    return $"No opponents next to {p.Label}.";

                case PowerId.MeteorStrike:
                    return CheckShot(s, p, a.GoalRow);

                case PowerId.Blink:
                {
                    if (!a.HasTarget) return "Blink needs a destination.";
                    var t = a.Target;
                    if (!Pitch.InBounds(t)) return $"{t} is off the pitch.";
                    int d = p.Pos.DistanceTo(t);
                    if (d < 1 || d > def.Range) return $"Blink reaches 1–{def.Range} tiles.";
                    if (!s.CanStandOn(p, t)) return "Keepers must stay in their penalty area.";
                    var occupant = s.PlayerAt(t);
                    if (occupant != null) return $"{t} is taken by {occupant.Label}.";
                    return null;
                }
                case PowerId.PhasePass:
                    return a.HasTarget ? CheckPass(s, p, a.Target) : "Phase Pass needs a target.";

                case PowerId.RiftTackle:
                {
                    var carrier = s.Carrier;
                    if (carrier == null || carrier.Side == p.Side) return "No opponent has the ball.";
                    if (p.Pos.DistanceTo(carrier.Pos) > def.Range) return $"{carrier.Label} is more than {def.Range} tiles away.";
                    if (!a.HasTarget)
                        return TryFindRiftLanding(s, p, carrier, out _) ? null : $"No free tile next to {carrier.Label}.";
                    var t = a.Target;
                    if (t.DistanceTo(carrier.Pos) != 1) return $"Land next to {carrier.Label}.";
                    if (!s.CanStandOn(p, t)) return "Keepers must stay in their penalty area.";
                    var occupant = s.PlayerAt(t);
                    if (occupant != null && occupant != p) return $"{t} is taken by {occupant.Label}.";
                    return null;
                }
                default:
                    return $"{def.Name} isn't implemented.";
            }
        }

        static string CheckUltimate(MatchState s, GameAction a)
        {
            var team = s.Team(s.ActiveSide);
            var def = Catalog.Ultimate(team.Species.Ultimate);
            if (def == null) return $"{team.Species.Name} have no ultimate.";
            if (team.UltimateUsed) return $"{def.Name} has already been used this match.";
            if (team.Flux < s.Config.MaxFlux) return $"{def.Name} needs full Flux ({team.Flux}/{s.Config.MaxFlux}).";
            if (!def.NeedsTwoPlayers) return null;

            if (a.PlayerId < 0 || a.PlayerId >= s.Players.Count || a.OtherPlayerId < 0 || a.OtherPlayerId >= s.Players.Count)
                return $"{def.Name} needs two of your players.";
            if (a.PlayerId == a.OtherPlayerId) return "Pick two different players.";
            var first = s.GetPlayer(a.PlayerId);
            var second = s.GetPlayer(a.OtherPlayerId);
            if (first.Side != s.ActiveSide || second.Side != s.ActiveSide) return "Both players must be yours.";
            if (!s.IsOnPitch(first) || !s.IsOnPitch(second)) return "Both players must be on the pitch.";
            if (!s.CanStandOn(first, second.Pos) || !s.CanStandOn(second, first.Pos)) return "Keepers must stay in their penalty area.";
            return null;
        }

        /// <summary>The free tile next to the carrier closest to the tackler (their own tile counts if already adjacent).</summary>
        internal static bool TryFindRiftLanding(MatchState s, Player tackler, Player carrier, out Tile landing)
        {
            landing = default;
            int best = int.MaxValue;
            foreach (var step in Pitch.Steps)
            {
                var t = carrier.Pos.Offset(step.X, step.Y);
                if (!s.CanStandOn(tackler, t)) continue;
                var occupant = s.PlayerAt(t);
                if (occupant != null && occupant != tackler) continue;
                int d = t.DistanceTo(tackler.Pos);
                if (d < best)
                {
                    best = d;
                    landing = t;
                }
            }
            return best != int.MaxValue;
        }
    }
}
