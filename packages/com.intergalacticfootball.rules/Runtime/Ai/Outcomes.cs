using System.Collections.Generic;

namespace IntergalacticFootball.Rules.Ai
{
    /// <summary>One possible result of an action and its probability.</summary>
    public readonly struct Outcome
    {
        public Outcome(MatchState state, double probability)
        {
            State = state;
            Probability = probability;
        }

        public readonly MatchState State;
        public readonly double Probability;
    }

    /// <summary>
    /// Enumerates the outcomes of an action without peeking at the match's real rolls: it replays the action on
    /// copies with forced rolls ("all succeed", then "fail at contest 1", "fail at contest 2", ...) and reads each
    /// contest's chance from the events.
    /// </summary>
    public static class Outcomes
    {
        public static List<Outcome> Enumerate(MatchState s, GameAction action, int maxOutcomes = 6)
        {
            var result = new List<Outcome>(2);
            Branch(s, action, new List<bool>(), 1.0, result, maxOutcomes);
            return result;
        }

        static readonly List<int> noChances = new List<int>();

        static void Branch(MatchState s, GameAction action, List<bool> prefix, double prefixProbability, List<Outcome> into, int maxOutcomes)
        {
            var copy = s.Clone();
            var outcome = MatchEngine.ApplyUnchecked(copy, action, new ScriptedRolls(true, prefix.ToArray()));

            List<int> chances = null;
            foreach (var e in outcome.Events)
            {
                if (e.Type != EventType.Contest) continue;
                if (chances == null) chances = new List<int>(4);
                chances.Add(e.Chance);
            }
            chances = chances ?? noChances;

            double p = prefixProbability;
            for (int j = prefix.Count; j < chances.Count; j++) p *= chances[j] / 100.0;
            into.Add(new Outcome(copy, p));

            double run = prefixProbability;
            for (int j = prefix.Count; j < chances.Count && into.Count < maxOutcomes; j++)
            {
                double fail = run * (1 - chances[j] / 100.0);
                if (fail > 1e-4)
                {
                    var next = new List<bool>(prefix);
                    while (next.Count < j) next.Add(true);
                    next.Add(false);
                    Branch(s, action, next, fail, into, maxOutcomes);
                }
                run *= chances[j] / 100.0;
            }
        }
    }
}
