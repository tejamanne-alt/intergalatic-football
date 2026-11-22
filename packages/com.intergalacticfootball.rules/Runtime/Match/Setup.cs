using System;
using System.Collections.Generic;

namespace IntergalacticFootball.Rules
{
    /// <summary>The strategic choices a team makes before kick-off.</summary>
    public sealed class TeamSetup
    {
        public TeamSetup(SpeciesDef species, Formation formation = null, IReadOnlyList<PowerId> loadout = null)
        {
            Species = species ?? throw new ArgumentNullException(nameof(species));
            Formation = formation ?? Formation.TwoThreeOne;
            Loadout = loadout ?? species.DefaultLoadout;
            if (Loadout.Count != Squad.Size) throw new ArgumentException("A loadout has one power per squad slot.", nameof(loadout));
            foreach (var power in Loadout)
            {
                if (power == PowerId.None) continue;
                bool allowed = false;
                foreach (var p in species.PowerPool) allowed |= p == power;
                if (!allowed) throw new ArgumentException($"{species.Name} can't bring {power}.", nameof(loadout));
            }
        }

        public SpeciesDef Species { get; }
        public Formation Formation { get; }
        public IReadOnlyList<PowerId> Loadout { get; }
    }

    public sealed class MatchSetup
    {
        public MatchSetup(TeamSetup home, TeamSetup away, ulong seed = 1, RulesConfig config = null)
        {
            Home = home ?? throw new ArgumentNullException(nameof(home));
            Away = away ?? throw new ArgumentNullException(nameof(away));
            Seed = seed;
            Config = config ?? new RulesConfig();
        }

        public TeamSetup Home { get; }
        public TeamSetup Away { get; }

        /// <summary>Seeds every roll of the match. Same setup + same actions = same match.</summary>
        public ulong Seed { get; }

        public RulesConfig Config { get; }

        public TeamSetup For(Side side) => side == Side.Home ? Home : Away;
    }
}
