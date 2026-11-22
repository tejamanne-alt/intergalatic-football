namespace IntergalacticFootball.Rules
{
    /// <summary>
    /// Every tunable number in the rules. Balance changes should land here (or in content data), not in the resolver.
    /// Treat an instance as read-only once a match has started: clones of a match share it.
    /// </summary>
    public sealed class RulesConfig
    {
        // Turn structure
        public int ActionsPerTurn { get; set; } = 3;
        public int TurnsPerHalfPerSide { get; set; } = 10;
        public int MaxFlux { get; set; } = 10;

        // Contest formula: chance = clamp(Base + PerStatPoint × (attack − defence) + modifiers, Min, Max)
        public int ContestBase { get; set; } = 50;
        public int ContestPerStatPoint { get; set; } = 8;
        public int ContestMin { get; set; } = 5;
        public int ContestMax { get; set; } = 95;

        // Pass lanes: a defender standing on the ball's path vs one only marking it
        public int LaneOnPathModifier { get; set; } = -10;
        public int LaneAdjacentModifier { get; set; } = 10;
        public int LongPassFreeDistance { get; set; } = 4;
        public int LongPassPenaltyPerTile { get; set; } = 3;

        // Shooting
        public int ShotLaneOnPathModifier { get; set; } = -20;
        public int ShotLaneAdjacentModifier { get; set; } = 0;

        /// <summary>Keepers get this on every save contest (negative = harder for the shooter).</summary>
        public int KeeperSaveModifier { get; set; } = -10;

        public int ShotFreeDistance { get; set; } = 3;
        public int ShotPenaltyPerTile { get; set; } = 4;

        /// <summary>The keeper contests a shot when within this many tiles of the goal-mouth tile it is aimed at.</summary>
        public int KeeperReach { get; set; } = 2;

        /// <summary>Bonus for the shooter when the keeper has to stretch (not right next to the target).</summary>
        public int KeeperStretchedModifier { get; set; } = 20;

        public int UnguardedAccuracy { get; set; } = 95;
        public int UnguardedPenaltyPerTile { get; set; } = 5;
        public int UnguardedMinAccuracy { get; set; } = 50;

        // Flux
        public int FluxPerPass { get; set; } = 1;
        public int FluxPerTackle { get; set; } = 2;
        public int FluxPerSave { get; set; } = 2;

        // Powers
        public int RiftTackleModifier { get; set; } = 10;
        public int GravityWellPull { get; set; } = 2;

        /// <summary>Tiles per Move action. Pace 3–5 → 2, 6–8 → 3, 9–10 → 4.</summary>
        public int MoveRange(int pace) => 1 + pace / 3;

        /// <summary>Maximum pass distance. Pass 4 → 6, 8 → 8.</summary>
        public int PassRange(int pass) => 4 + pass / 2;

        /// <summary>Maximum shot distance, measured to the goal tile. Shot 8 → 7.</summary>
        public int ShotRange(int shot) => 3 + shot / 2;

        public int TurnsPerHalf => TurnsPerHalfPerSide * 2;

        public int TotalTurns => TurnsPerHalfPerSide * 4;
    }
}
