using System.Collections.Generic;

namespace IntergalacticFootball.Rules
{
    /// <summary>A way to reach a destination with one Move action.</summary>
    public sealed class MovePlan
    {
        internal MovePlan(Tile destination, Tile[] path, double successProbability, int contests)
        {
            Destination = destination;
            Path = path;
            SuccessProbability = successProbability;
            Contests = contests;
        }

        public Tile Destination { get; }
        public IReadOnlyList<Tile> Path { get; }

        /// <summary>For a ball carrier: chance of arriving with the ball. Always 1 off the ball.</summary>
        public double SuccessProbability { get; }

        /// <summary>Dribble contests on the way.</summary>
        public int Contests { get; }
    }

    /// <summary>
    /// Where a player can go with one Move. Off the ball, zones don't matter, so this is the shortest path.
    /// With the ball, it is the path most likely to arrive (then the shortest): stepping into an enemy zone
    /// means a dribble contest and ends the move there.
    /// </summary>
    public static class Pathing
    {
        /// <summary>
        /// Every reachable destination with its best path, most forward first (for the player's side), then by row.
        /// The order is the same for both sides, so nothing downstream favours Home or Away.
        /// </summary>
        public static List<MovePlan> Reachable(MatchState s, Player p)
        {
            var plans = s.BallCarrierId == p.Id ? CarrierPlans(s, p) : OffBallPlans(s, p);
            var side = p.Side;
            plans.Sort((a, b) =>
            {
                int pa = Pitch.Progress(side, a.Destination), pb = Pitch.Progress(side, b.Destination);
                return pa != pb ? pb.CompareTo(pa) : a.Destination.Y.CompareTo(b.Destination.Y);
            });
            return plans;
        }

        /// <summary>The best path to one tile, or null if it can't be reached this move.</summary>
        public static MovePlan Find(MatchState s, Player p, Tile destination)
        {
            foreach (var plan in Reachable(s, p))
                if (plan.Destination == destination) return plan;
            return null;
        }

        static List<MovePlan> OffBallPlans(MatchState s, Player p)
        {
            int range = s.MoveRange(p);
            var parent = new Dictionary<Tile, Tile>();
            var depth = new Dictionary<Tile, int> { [p.Pos] = 0 };
            var queue = new Queue<Tile>();
            queue.Enqueue(p.Pos);
            var order = new List<Tile>();

            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                int d = depth[cur];
                if (d >= range) continue;
                if (cur != p.Pos && s.BallIsLoose && s.LooseBallTile == cur) continue; // picking up ends the move
                foreach (var step in Pitch.Steps)
                {
                    var n = cur.Offset(step.X, step.Y);
                    if (depth.ContainsKey(n) || !s.CanStandOn(p, n) || s.PlayerAt(n) != null) continue;
                    depth[n] = d + 1;
                    parent[n] = cur;
                    queue.Enqueue(n);
                    order.Add(n);
                }
            }

            var plans = new List<MovePlan>(order.Count);
            foreach (var dest in order)
            {
                var path = new Tile[depth[dest]];
                var t = dest;
                for (int i = path.Length - 1; i >= 0; i--)
                {
                    path[i] = t;
                    t = parent[t];
                }
                plans.Add(new MovePlan(dest, path, 1.0, 0));
            }
            return plans;
        }

        sealed class Best
        {
            public Tile[] Path;
            public double P;
            public int Contests;
        }

        static List<MovePlan> CarrierPlans(MatchState s, Player p)
        {
            int range = s.MoveRange(p);
            int ignores = s.Team(p.Side).Species.Passive == Passive.Phasing ? 1 : 0;
            var best = new Dictionary<Tile, Best>();
            var path = new List<Tile>(range);
            Search(s, p, p.Pos, range, 1.0, 0, ignores, 0, path, best);

            var plans = new List<MovePlan>(best.Count);
            foreach (var kv in best) plans.Add(new MovePlan(kv.Key, kv.Value.Path, kv.Value.P, kv.Value.Contests));
            return plans;
        }

        static void Search(MatchState s, Player p, Tile cur, int stepsLeft, double prob, int handled, int ignores, int contests, List<Tile> path, Dictionary<Tile, Best> best)
        {
            foreach (var step in Pitch.Steps)
            {
                var n = cur.Offset(step.X, step.Y);
                if (n == p.Pos || path.Contains(n) || !s.CanStandOn(p, n) || s.PlayerAt(n) != null) continue;

                double np = prob;
                int nh = handled, ni = ignores, nc = contests;
                foreach (var e in s.PlayersOf(p.Side.Opponent()))
                {
                    int bit = 1 << e.Id;
                    if ((nh & bit) != 0 || !Contests.InZone(e, n)) continue;
                    nh |= bit;
                    if (ni > 0)
                    {
                        ni--;
                        continue;
                    }
                    np *= Contests.Dribble(s, p, e) / 100.0;
                    nc++;
                }

                path.Add(n);
                if (!best.TryGetValue(n, out var b) || np > b.P + 1e-9 || (np > b.P - 1e-9 && path.Count < b.Path.Length))
                    best[n] = new Best { Path = path.ToArray(), P = np, Contests = nc };
                bool stopped = nc > contests; // stepping into a zone ends the move
                if (stepsLeft > 1 && !stopped) Search(s, p, n, stepsLeft - 1, np, nh, ni, nc, path, best);
                path.RemoveAt(path.Count - 1);
            }
        }
    }
}
