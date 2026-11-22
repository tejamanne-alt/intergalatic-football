using System;
using System.Collections.Generic;

namespace IntergalacticFootball.Rules
{
    /// <summary>
    /// Contest odds as pure functions of the state. The resolver, the previews, path-finding and the AI all use these,
    /// so the number shown before committing is the number that gets rolled.
    /// </summary>
    public static class Contests
    {
        /// <summary>chance = clamp(Base + PerStatPoint × (attack − defence) + modifier, Min, Max)</summary>
        public static int Chance(RulesConfig c, int attack, int defence, int modifier) =>
            Math.Max(c.ContestMin, Math.Min(c.ContestMax, c.ContestBase + c.ContestPerStatPoint * (attack - defence) + modifier));

        /// <summary>Carrier stepping into a defender's zone: carrier Pace vs defender Tackle.</summary>
        public static int Dribble(MatchState s, Player carrier, Player defender) =>
            Chance(s.Config, s.EffectivePace(carrier), defender.Stats.Tackle, 0);

        /// <summary>Tackle action: tackler Tackle vs carrier Pace.</summary>
        public static int Tackle(MatchState s, Player tackler, Player carrier, int modifier = 0) =>
            Chance(s.Config, tackler.Stats.Tackle, s.EffectivePace(carrier), modifier);

        /// <summary>Is <paramref name="t"/> inside <paramref name="p"/>'s zone (their tile and the 8 around it)?</summary>
        public static bool InZone(Player p, Tile t) => Pitch.InBounds(p.Pos) && p.Pos.DistanceTo(t) <= 1;

        /// <summary>
        /// Interception contests for a pass, in the order the ball meets them. Each opponent whose zone the ball
        /// passes through (including the receiver's tile) contests once.
        /// </summary>
        public static List<ContestInfo> Pass(MatchState s, Player passer, Tile target, bool phased)
        {
            var result = new List<ContestInfo>();
            if (phased) return result;
            var cfg = s.Config;
            int distance = passer.Pos.DistanceTo(target);
            int longPass = distance > cfg.LongPassFreeDistance ? -cfg.LongPassPenaltyPerTile * (distance - cfg.LongPassFreeDistance) : 0;
            int handled = 0;
            var path = Pitch.Line(passer.Pos, target);
            foreach (var t in path)
            {
                foreach (var e in s.PlayersOf(passer.Side.Opponent()))
                {
                    int bit = 1 << e.Id;
                    if ((handled & bit) != 0 || !InZone(e, t)) continue;
                    handled |= bit;
                    int lane = path.Contains(e.Pos) ? cfg.LaneOnPathModifier : cfg.LaneAdjacentModifier;
                    result.Add(new ContestInfo(ContestKind.Interception, passer.Id, e.Id, Chance(cfg, passer.Stats.Pass, e.Stats.Tackle, lane + longPass)));
                }
            }
            return result;
        }

        /// <summary>
        /// Contests for a shot at a goal row: blocks by outfield defenders along the ball's path, then either
        /// a keeper save (keeper within reach of the goal mouth) or an accuracy roll at an unguarded goal.
        /// </summary>
        public static List<ContestInfo> Shot(MatchState s, Player shooter, int row, bool unblockable)
        {
            var result = new List<ContestInfo>();
            var cfg = s.Config;
            var defending = shooter.Side.Opponent();
            var goal = Pitch.GoalTile(defending, row);
            int distance = shooter.Pos.DistanceTo(goal);

            if (!unblockable)
            {
                int handled = 0;
                var path = Pitch.Line(shooter.Pos, goal);
                foreach (var t in path)
                {
                    if (!Pitch.InBounds(t)) continue;
                    foreach (var e in s.PlayersOf(defending))
                    {
                        int bit = 1 << e.Id;
                        if (e.IsKeeper || (handled & bit) != 0 || !InZone(e, t)) continue;
                        handled |= bit;
                        int lane = path.Contains(e.Pos) ? cfg.ShotLaneOnPathModifier : cfg.ShotLaneAdjacentModifier;
                        result.Add(new ContestInfo(ContestKind.Block, shooter.Id, e.Id, Chance(cfg, shooter.Stats.Shot, e.Stats.Tackle, lane)));
                    }
                }
            }

            int beyond = distance - cfg.ShotFreeDistance;
            var keeper = s.Keeper(defending);
            if (s.IsOnPitch(keeper))
            {
                int reach = keeper.Pos.DistanceTo(Pitch.GoalMouth(defending, row));
                if (reach <= cfg.KeeperReach)
                {
                    int modifier = cfg.KeeperSaveModifier - cfg.ShotPenaltyPerTile * beyond + (reach > 1 ? cfg.KeeperStretchedModifier : 0);
                    result.Add(new ContestInfo(ContestKind.Save, shooter.Id, keeper.Id, Chance(cfg, shooter.Stats.Shot, keeper.Stats.Tackle, modifier)));
                    return result;
                }
            }

            int accuracy = cfg.UnguardedAccuracy - cfg.UnguardedPenaltyPerTile * Math.Max(0, beyond);
            accuracy = Math.Max(cfg.UnguardedMinAccuracy, Math.Min(cfg.ContestMax, accuracy));
            result.Add(new ContestInfo(ContestKind.Accuracy, shooter.Id, -1, accuracy));
            return result;
        }

        public static double Probability(IReadOnlyList<ContestInfo> contests)
        {
            double p = 1.0;
            for (int i = 0; i < contests.Count; i++) p *= contests[i].Chance / 100.0;
            return p;
        }

        public static bool ShotInRange(MatchState s, Player shooter, int row) =>
            shooter.Pos.DistanceTo(Pitch.GoalTile(shooter.Side.Opponent(), row)) <= s.ShotRange(shooter);

        /// <summary>Best goal chance across the three goal rows, or 0 when out of range.</summary>
        public static double BestGoalProbability(MatchState s, Player shooter, out int bestRow, bool unblockable = false)
        {
            bestRow = Pitch.GoalCentreRow;
            double best = 0;
            if (!s.IsOnPitch(shooter)) return 0;
            for (int row = Pitch.GoalTop; row <= Pitch.GoalBottom; row++)
            {
                if (!ShotInRange(s, shooter, row)) continue;
                double p = Probability(Shot(s, shooter, row, unblockable));
                if (p > best)
                {
                    best = p;
                    bestRow = row;
                }
            }
            return best;
        }
    }
}
