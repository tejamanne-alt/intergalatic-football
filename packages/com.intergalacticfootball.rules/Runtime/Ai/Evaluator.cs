using System;

namespace IntergalacticFootball.Rules.Ai
{
    /// <summary>
    /// Scores a position from one side's point of view. A goal is worth 1000; everything else is a rough
    /// estimate of how likely the position is to turn into one.
    /// </summary>
    public static class Evaluator
    {
        const double Goal = 1000;
        const double PossessionBase = 40;
        const double PerProgressTile = 5;

        public static double Evaluate(MatchState s, Side me)
        {
            var opp = me.Opponent();
            var mine = s.Team(me);
            var theirs = s.Team(opp);
            double goalDiff = mine.Score - theirs.Score;
            if (s.IsOver) return 100000.0 * Math.Sign(goalDiff) + Goal * goalDiff;

            double v = Goal * goalDiff;
            v += 2.0 * (mine.Flux - theirs.Flux);
            if (s.UltimateReady(me)) v += 10;
            if (s.UltimateReady(opp)) v -= 10;
            if (s.ActiveSide == me) v += 6.0 * s.ActionsLeft;

            var carrier = s.Carrier;
            if (carrier != null)
            {
                double pv = PossessionValue(s, carrier);
                v += carrier.Side == me ? pv : -pv;
            }
            else
            {
                v += LooseBallValue(s, me);
            }

            v += Shape(s, me) - Shape(s, opp);
            return v;
        }

        static double SpotValue(Side side, Tile t) => PossessionBase + PerProgressTile * Pitch.Progress(side, t);

        /// <summary>Value of <paramref name="c"/>'s side holding the ball where it is.</summary>
        static double PossessionValue(MatchState s, Player c)
        {
            var side = c.Side;
            double keep = SpotValue(side, c.Pos);
            double lose = SpotValue(side.Opponent(), c.Pos);
            double shot = Contests.BestGoalProbability(s, c, out _);
            double risk = TackleRisk(s, c);

            if (s.ActiveSide == side && s.ActionsLeft > 0)
            {
                // They can still act this turn: the shot is on now, and the risk only bites later.
                double r = 0.35 * risk;
                return keep * (1 - r) - lose * r + 0.85 * Goal * shot;
            }
            return keep * (1 - risk) - lose * risk + 0.3 * Goal * shot * (1 - risk);
        }

        /// <summary>Rough chance the opponents win the ball from <paramref name="c"/> on their next turn.</summary>
        static double TackleRisk(MatchState s, Player c)
        {
            double t1 = 0, t2 = 0;
            foreach (var e in s.PlayersOf(c.Side.Opponent()))
            {
                if (!s.IsOnPitch(e)) continue;
                int d = e.Pos.DistanceTo(c.Pos);
                int reach = s.Config.MoveRange(e.Stats.Pace);
                double w;
                if (d <= 1) w = 1.0;
                else if (d - 1 <= reach) w = 0.75;
                else if (d - 1 <= 2 * reach) w = 0.3;
                else continue;
                double t = w * Contests.Tackle(s, e, c) / 100.0;
                if (t > t1)
                {
                    t2 = t1;
                    t1 = t;
                }
                else if (t > t2)
                {
                    t2 = t;
                }
            }
            return Math.Min(0.9, 1 - (1 - t1) * (1 - t2));
        }

        static double LooseBallValue(MatchState s, Side me)
        {
            var ball = s.LooseBallTile;
            var active = s.ActiveSide;
            int activeN = ActionsToReach(s, active, ball);
            int otherN = ActionsToReach(s, active.Opponent(), ball);

            Side owner;
            double confidence;
            if (activeN <= s.ActionsLeft)
            {
                owner = active;
                confidence = 0.85;
            }
            else if (otherN <= s.Config.ActionsPerTurn)
            {
                owner = active.Opponent();
                confidence = 0.7;
            }
            else
            {
                int mineN = active == me ? activeN : otherN;
                int theirsN = active == me ? otherN : activeN;
                owner = mineN <= theirsN ? me : me.Opponent();
                confidence = 0.4;
            }

            double v = confidence * SpotValue(owner, ball);
            return owner == me ? v : -v;
        }

        static int ActionsToReach(MatchState s, Side side, Tile t)
        {
            int best = 99;
            foreach (var p in s.PlayersOf(side))
            {
                if (!s.IsOnPitch(p) || !s.CanStandOn(p, t)) continue;
                int d = p.Pos.DistanceTo(t);
                int r = Math.Max(1, s.MoveRange(p));
                int n = (d + r - 1) / r;
                if (n < best) best = n;
            }
            return best;
        }

        /// <summary>Positional quality for one side: keeper on the line, cover, pressing, marking, passing options.</summary>
        static double Shape(MatchState s, Side side)
        {
            double v = 0;
            var keeper = s.Keeper(side);
            if (s.IsOnPitch(keeper)) v -= 4 * keeper.Pos.DistanceTo(Pitch.GoalMouth(side, Pitch.GoalCentreRow));

            var carrier = s.Carrier;
            var opp = side.Opponent();
            var ball = s.BallTile;
            int ballProgress = Pitch.Progress(side, ball);
            bool attacking = carrier != null && carrier.Side == side;

            // Off-ball shape: each outfield player has a home spot from the formation that slides up and
            // down the pitch with the ball (further up when in possession) and leans toward its row.
            var formation = s.Team(side).Formation;
            int shift = (ballProgress - 7) * 2 / 3 + (attacking ? 2 : 0);
            foreach (var p in s.PlayersOf(side))
            {
                if (p.IsKeeper || !s.IsOnPitch(p) || p == carrier) continue;
                var slot = formation.Slots[p.Slot]; // home orientation: x is progress
                int progress = Math.Max(0, Math.Min(Pitch.Width - 1, slot.X + shift));
                int row = slot.Y + (ball.Y - slot.Y) / 3;
                var spot = new Tile(side == Side.Home ? progress : Pitch.Width - 1 - progress, row);
                v -= 0.8 * p.Pos.DistanceTo(spot);
            }

            if (!attacking)
            {
                int goalSide = 0, pressing = 0;
                foreach (var p in s.PlayersOf(side))
                {
                    if (p.IsKeeper || !s.IsOnPitch(p)) continue;
                    if (Pitch.Progress(side, p.Pos) < ballProgress) goalSide++;
                    if (carrier != null && p.Pos.DistanceTo(carrier.Pos) == 1) pressing++;
                }
                v += 3 * Math.Min(goalSide, 4) + 6 * Math.Min(pressing, 2);

                foreach (var q in s.PlayersOf(opp))
                {
                    if (q.IsKeeper || !s.IsOnPitch(q) || Pitch.Progress(opp, q.Pos) < 9) continue;
                    bool marked = false;
                    foreach (var p in s.PlayersOf(side))
                        if (s.IsOnPitch(p) && p.Pos.DistanceTo(q.Pos) <= 1) marked = true;
                    if (!marked) v -= 5;
                }
            }
            else
            {
                int options = 0;
                int range = s.PassRange(carrier);
                foreach (var p in s.PlayersOf(side))
                {
                    if (p == carrier || p.IsKeeper || !s.IsOnPitch(p)) continue;
                    if (Pitch.Progress(side, p.Pos) < ballProgress - 1 || p.Pos.DistanceTo(carrier.Pos) > range) continue;
                    bool marked = false;
                    foreach (var e in s.PlayersOf(opp))
                        if (Contests.InZone(e, p.Pos)) marked = true;
                    if (!marked) options++;
                }
                v += 4 * Math.Min(options, 3);
            }
            return v;
        }
    }
}
