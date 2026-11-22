using System;
using System.Collections.Generic;

namespace IntergalacticFootball.Rules
{
    /// <summary>
    /// The rules entry point. Validate an action, preview its odds, then apply it.
    /// The same calls drive local matches, the AI, puzzles, replays and server-side turn checks.
    /// </summary>
    public static class MatchEngine
    {
        /// <summary>Null when the action is legal, otherwise a readable reason.</summary>
        public static string Validate(MatchState state, GameAction action) => Validation.Check(state, action);

        public static bool IsLegal(MatchState state, GameAction action) => Validation.Check(state, action) == null;

        /// <summary>
        /// Applies a legal action. Rolls come from the match's own seeded stream unless <paramref name="rolls"/> is given.
        /// Throws <see cref="InvalidOperationException"/> for an illegal action.
        /// </summary>
        public static ActionResult Apply(MatchState state, GameAction action, IRollSource rolls = null)
        {
            var error = Validation.Check(state, action);
            if (error != null) throw new InvalidOperationException(error);
            return ApplyUnchecked(state, action, rolls);
        }

        /// <summary>For callers that generated the action from the legal set themselves (the AI).</summary>
        internal static ActionResult ApplyUnchecked(MatchState state, GameAction action, IRollSource rolls)
        {
            var events = new List<MatchEvent>();
            bool ok = new Resolver(state, rolls, events).Run(action);
            return new ActionResult(action, events, ok);
        }

        /// <summary>
        /// The contests an action will face and the chance that it fully succeeds. Does not change the state
        /// or advance its roll stream.
        /// </summary>
        public static ActionPreview Preview(MatchState state, GameAction action)
        {
            var error = Validation.Check(state, action);
            if (error != null) return ActionPreview.Illegal(error);
            var result = ApplyUnchecked(state.Clone(), action, ScriptedRolls.AlwaysSucceed());
            var contests = new List<ContestInfo>();
            foreach (var e in result.Events)
                if (e.Type == EventType.Contest)
                    contests.Add(new ContestInfo(e.ContestKind, e.PlayerId, e.OtherPlayerId, e.Chance));
            return ActionPreview.Legal(contests, Contests.Probability(contests));
        }
    }
}
