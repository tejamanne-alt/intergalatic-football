using System;
using System.Collections.Generic;

namespace IntergalacticFootball.Rules.Ai
{
    public enum AiDifficulty
    {
        Easy,
        Medium,
        Hard,
    }

    public sealed class AiSettings
    {
        /// <summary>How many of its own actions the AI looks ahead (1–3).</summary>
        public int Depth { get; set; } = 2;

        /// <summary>How many of the best-looking actions get the deeper look at each level.</summary>
        public int BeamWidth { get; set; } = 5;

        /// <summary>Random noise added to first-level scores, so easier AIs make human-looking mistakes.</summary>
        public double Noise { get; set; }

        /// <summary>Cap on outcomes considered per action.</summary>
        public int MaxOutcomes { get; set; } = 6;

        public static AiSettings For(AiDifficulty difficulty)
        {
            switch (difficulty)
            {
                case AiDifficulty.Easy: return new AiSettings { Depth = 1, BeamWidth = 1, Noise = 40 };
                case AiDifficulty.Hard: return new AiSettings { Depth = 3, BeamWidth = 6 };
                default: return new AiSettings { Depth = 2, BeamWidth = 5 };
            }
        }
    }

    public sealed class ScoredAction
    {
        public ScoredAction(GameAction action, double value)
        {
            Action = action;
            Value = value;
        }

        public GameAction Action { get; }

        /// <summary>Expected evaluation after the action (and the best follow-ups within the look-ahead).</summary>
        public double Value { get; }
    }

    /// <summary>
    /// The on-device opponent. Scores every legal action by its expected outcome, then looks further ahead
    /// across the rest of its 3 actions for the most promising ones. Difficulty is look-ahead depth, beam
    /// width and noise. It never reads the match's real rolls.
    /// </summary>
    public sealed class AiPlayer
    {
        readonly AiSettings settings;
        Pcg32 rng;

        public AiPlayer(AiDifficulty difficulty, ulong seed = 7)
            : this(AiSettings.For(difficulty), seed)
        {
        }

        public AiPlayer(AiSettings settings, ulong seed = 7)
        {
            this.settings = settings ?? new AiSettings();
            rng = new Pcg32(seed, 0x51ed270b27d0e4a1UL);
        }

        public AiSettings Settings => settings;

        public GameAction ChooseAction(MatchState state)
        {
            var ranked = Rank(state);
            return ranked.Count > 0 ? ranked[0].Action : GameAction.EndTurn();
        }

        /// <summary>Every legal action, best first. The top few carry look-ahead values; the rest are one-ply.</summary>
        public List<ScoredAction> Rank(MatchState state)
        {
            var me = state.ActiveSide;
            var nodes = Expand(state, me);
            if (settings.Noise > 0)
                foreach (var n in nodes) n.Value += (rng.NextDouble() * 2 - 1) * settings.Noise;
            nodes.Sort(Node.Compare);

            int depth = Math.Min(settings.Depth, Math.Max(1, state.ActionsLeft));
            if (depth > 1)
            {
                int k = Math.Min(settings.BeamWidth, nodes.Count);
                for (int i = 0; i < k; i++) nodes[i].Value = Deep(nodes[i], me, depth - 1);
                nodes.Sort(0, k, Node.Comparer);
            }

            var result = new List<ScoredAction>(nodes.Count);
            foreach (var n in nodes) result.Add(new ScoredAction(n.Action, n.Value));
            return result;
        }

        sealed class Node
        {
            public GameAction Action;
            public List<Outcome> Outcomes;
            public double Value;
            public int Index;

            public static int Compare(Node a, Node b)
            {
                int c = b.Value.CompareTo(a.Value);
                return c != 0 ? c : a.Index.CompareTo(b.Index);
            }

            public static readonly IComparer<Node> Comparer = Comparer<Node>.Create(Compare);
        }

        double Deep(Node node, Side me, int depth)
        {
            double total = 0, weight = 0;
            foreach (var o in node.Outcomes)
            {
                total += o.Probability * Value(o.State, me, depth);
                weight += o.Probability;
            }
            return weight > 0 ? total / weight : node.Value;
        }

        double Value(MatchState s, Side me, int depth)
        {
            if (s.IsOver || s.ActiveSide != me || depth <= 0) return Evaluator.Evaluate(s, me);
            var nodes = Expand(s, me);
            if (nodes.Count == 0) return Evaluator.Evaluate(s, me);
            nodes.Sort(Node.Compare);
            if (depth == 1) return nodes[0].Value;

            int k = Math.Min(settings.BeamWidth, nodes.Count);
            double best = double.NegativeInfinity;
            for (int i = 0; i < k; i++) best = Math.Max(best, Deep(nodes[i], me, depth - 1));
            return best;
        }

        List<Node> Expand(MatchState s, Side me)
        {
            var actions = ActionGenerator.ForActiveSide(s, compact: true);
            var nodes = new List<Node>(actions.Count);
            for (int i = 0; i < actions.Count; i++)
            {
                var outcomes = Outcomes.Enumerate(s, actions[i], settings.MaxOutcomes);
                double total = 0, weight = 0;
                foreach (var o in outcomes)
                {
                    total += o.Probability * Evaluator.Evaluate(o.State, me);
                    weight += o.Probability;
                }
                nodes.Add(new Node { Action = actions[i], Outcomes = outcomes, Value = weight > 0 ? total / weight : double.NegativeInfinity, Index = i });
            }
            return nodes;
        }
    }
}
