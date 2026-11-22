using System.Collections.Generic;

namespace IntergalacticFootball.Rules
{
    public enum EventType
    {
        TurnStarted,
        TurnEnded,
        KickOff,
        HalfTime,
        FullTime,

        /// <summary>A player moved along <see cref="MatchEvent.Path"/> (one segment; a move with contests has several).</summary>
        Moved,

        /// <summary>Phasing: the carrier ignored this enemy's zone.</summary>
        ZoneIgnored,

        /// <summary>A roll. <see cref="MatchEvent.PlayerId"/> is the side that needed to succeed.</summary>
        Contest,

        /// <summary>A defender won the ball from a dribbling carrier.</summary>
        Dispossessed,

        /// <summary>Anchored: the carrier lost the dribble contest but kept the ball and stopped.</summary>
        HeldOn,

        PickedUp,
        PassMade,
        PassCompleted,
        PassIntoSpace,
        Intercepted,
        TackleWon,
        TackleFailed,
        Shot,
        Goal,
        Saved,
        Blocked,
        Missed,
        PowerUsed,
        UltimateUsed,
        Teleported,
        Pushed,
        PushBlocked,
        BallDropped,
        BallPulled,
        Swapped,
        PaceHalved,
        FluxGained,
    }

    /// <summary>What happened, in order, for animation, logs and stats.</summary>
    public sealed class MatchEvent
    {
        public EventType Type { get; internal set; }
        public int PlayerId { get; internal set; } = -1;
        public int OtherPlayerId { get; internal set; } = -1;
        public Side Side { get; internal set; }
        public Tile From { get; internal set; }
        public Tile To { get; internal set; }
        public IReadOnlyList<Tile> Path { get; internal set; }
        public ContestKind ContestKind { get; internal set; }

        /// <summary>Contest: success chance in percent.</summary>
        public int Chance { get; internal set; }

        /// <summary>Contest: the d100 roll (0–99). Success when Roll &lt; Chance.</summary>
        public int Roll { get; internal set; }

        public bool Success { get; internal set; }

        /// <summary>Flux gained, the new score after a goal, or a turn index.</summary>
        public int Amount { get; internal set; }

        public PowerId Power { get; internal set; }
        public UltimateId Ultimate { get; internal set; }

        public override string ToString() => Type.ToString();
    }

    public sealed class ActionResult
    {
        internal ActionResult(GameAction action, IReadOnlyList<MatchEvent> events, bool succeeded)
        {
            Action = action;
            Events = events;
            Succeeded = succeeded;
        }

        public GameAction Action { get; }
        public IReadOnlyList<MatchEvent> Events { get; }

        /// <summary>
        /// Whether the action did what it set out to: the move arrived, the pass reached its target,
        /// the shot went in, the tackle won the ball.
        /// </summary>
        public bool Succeeded { get; }
    }

    public readonly struct ContestInfo
    {
        public ContestInfo(ContestKind kind, int attackerId, int defenderId, int chance)
        {
            Kind = kind;
            AttackerId = attackerId;
            DefenderId = defenderId;
            Chance = chance;
        }

        public readonly ContestKind Kind;

        /// <summary>The player who needs to succeed (carrier, passer, shooter, tackler).</summary>
        public readonly int AttackerId;

        /// <summary>The opposing player, or -1 for an accuracy roll.</summary>
        public readonly int DefenderId;

        /// <summary>The attacker's success chance in percent.</summary>
        public readonly int Chance;
    }

    /// <summary>The odds shown before the player commits.</summary>
    public sealed class ActionPreview
    {
        ActionPreview(string error, IReadOnlyList<ContestInfo> contests, double probability)
        {
            Error = error;
            Contests = contests ?? new ContestInfo[0];
            SuccessProbability = probability;
        }

        public bool IsLegal => Error == null;
        public string Error { get; }

        /// <summary>Every contest the action runs into if each one before it succeeds.</summary>
        public IReadOnlyList<ContestInfo> Contests { get; }

        /// <summary>Chance that every contest succeeds, 0–1.</summary>
        public double SuccessProbability { get; }

        public int SuccessPercent => (int)System.Math.Round(SuccessProbability * 100.0);

        internal static ActionPreview Illegal(string error) => new ActionPreview(error, null, 0);

        internal static ActionPreview Legal(IReadOnlyList<ContestInfo> contests, double probability) =>
            new ActionPreview(null, contests, probability);
    }
}
