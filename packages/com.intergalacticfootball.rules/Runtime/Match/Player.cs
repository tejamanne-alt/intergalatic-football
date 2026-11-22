namespace IntergalacticFootball.Rules
{
    public sealed class Player
    {
        internal Player(int id, Side side, int slot, Role role, string label, Stats stats, PowerId power)
        {
            Id = id;
            Side = side;
            Slot = slot;
            Role = role;
            Label = label;
            Stats = stats;
            Power = power;
            Pos = Tile.Bench;
        }

        /// <summary>0–6 for Home, 7–13 for Away, in squad-slot order.</summary>
        public int Id { get; }

        public Side Side { get; }
        public int Slot { get; }
        public Role Role { get; }
        public string Label { get; }

        /// <summary>Base stats. Use <see cref="MatchState.EffectivePace"/> for Pace, which effects can change.</summary>
        public Stats Stats { get; }

        public PowerId Power { get; }

        public Tile Pos { get; internal set; }

        /// <summary>Own turns left before the power is usable again.</summary>
        public int PowerCooldown { get; internal set; }

        public bool IsKeeper => Role == Role.Keeper;

        internal Player Clone() => (Player)MemberwiseClone();

        public override string ToString() => Label;
    }

    public sealed class TeamState
    {
        internal TeamState(Side side, TeamSetup setup)
        {
            Side = side;
            Setup = setup;
            PaceHalvedThroughTurn = -1;
        }

        public Side Side { get; }
        public TeamSetup Setup { get; }
        public SpeciesDef Species => Setup.Species;
        public Formation Formation => Setup.Formation;

        public int Score { get; internal set; }
        public int Flux { get; internal set; }
        public bool UltimateUsed { get; internal set; }

        /// <summary>This team's Pace is halved while the match turn index is at or below this value (Crush).</summary>
        public int PaceHalvedThroughTurn { get; internal set; }

        internal TeamState Clone() => (TeamState)MemberwiseClone();
    }
}
