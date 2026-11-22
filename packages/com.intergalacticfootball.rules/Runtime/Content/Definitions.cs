using System;
using System.Collections.Generic;

namespace IntergalacticFootball.Rules
{
    /// <summary>What a power needs from the player when it is used.</summary>
    public enum PowerTarget
    {
        None,

        /// <summary>A tile (Blink destination; optional landing tile for Rift Tackle).</summary>
        Tile,

        /// <summary>A pass target: a teammate's tile or an empty tile.</summary>
        PassTarget,

        /// <summary>A goal row, 3–5.</summary>
        GoalRow,
    }

    /// <summary>
    /// A player power. Effects are implemented in the resolver; everything numeric is data here.
    /// In Unity these map 1:1 to ScriptableObjects.
    /// </summary>
    public sealed class PowerDef
    {
        public PowerDef(PowerId id, string name, string description, int fluxCost, int cooldown, int range, PowerTarget target)
        {
            Id = id;
            Name = name;
            Description = description;
            FluxCost = fluxCost;
            Cooldown = cooldown;
            Range = range;
            Target = target;
        }

        public PowerId Id { get; }
        public string Name { get; }
        public string Description { get; }
        public int FluxCost { get; }

        /// <summary>Own turns before the power can be used again. 2 = skip the next turn, usable the one after.</summary>
        public int Cooldown { get; }

        /// <summary>Power-specific reach in tiles; 0 when the power uses the player's normal pass or shot range.</summary>
        public int Range { get; }

        public PowerTarget Target { get; }
    }

    public sealed class UltimateDef
    {
        public UltimateDef(UltimateId id, string name, string description, bool needsTwoPlayers)
        {
            Id = id;
            Name = name;
            Description = description;
            NeedsTwoPlayers = needsTwoPlayers;
        }

        public UltimateId Id { get; }
        public string Name { get; }
        public string Description { get; }
        public bool NeedsTwoPlayers { get; }
    }

    public sealed class SpeciesDef
    {
        readonly Stats[] statsByRole;

        public SpeciesDef(
            string id,
            string name,
            string style,
            Passive passive,
            string passiveName,
            string passiveDescription,
            Stats[] statsByRole,
            IReadOnlyList<PowerId> powerPool,
            IReadOnlyList<PowerId> defaultLoadout,
            UltimateId ultimate)
        {
            if (statsByRole == null || statsByRole.Length != 5) throw new ArgumentException("Need stats for all 5 roles.", nameof(statsByRole));
            if (defaultLoadout == null || defaultLoadout.Count != Squad.Size) throw new ArgumentException("Need one power per squad slot.", nameof(defaultLoadout));
            Id = id;
            Name = name;
            Style = style;
            Passive = passive;
            PassiveName = passiveName;
            PassiveDescription = passiveDescription;
            this.statsByRole = statsByRole;
            PowerPool = powerPool;
            DefaultLoadout = defaultLoadout;
            Ultimate = ultimate;
        }

        public string Id { get; }
        public string Name { get; }
        public string Style { get; }
        public Passive Passive { get; }
        public string PassiveName { get; }
        public string PassiveDescription { get; }

        /// <summary>The powers this species can bring.</summary>
        public IReadOnlyList<PowerId> PowerPool { get; }

        /// <summary>One power per squad slot (see <see cref="Squad"/>).</summary>
        public IReadOnlyList<PowerId> DefaultLoadout { get; }

        public UltimateId Ultimate { get; }

        public Stats StatsFor(Role role) => statsByRole[(int)role];

        public override string ToString() => Name;
    }

    /// <summary>The seven squad slots. Every squad has the same roles in the same slots.</summary>
    public static class Squad
    {
        public const int Size = 7;
        public const int KeeperSlot = 0;
        public const int StrikerSlot = 6;

        static readonly Role[] roles =
        {
            Role.Keeper, Role.Anchor, Role.Anchor, Role.Playmaker, Role.Playmaker, Role.Runner, Role.Striker,
        };

        static readonly string[] labels = { "K", "A", "B", "P", "Q", "R", "S" };

        public static Role RoleOf(int slot) => roles[slot];

        /// <summary>Short label for boards and logs: upper case for Home, lower case for Away.</summary>
        public static string LabelOf(int slot, Side side) => side == Side.Home ? labels[slot] : labels[slot].ToLowerInvariant();
    }

    /// <summary>Kick-off positions for the seven slots, in home orientation (own half is x 0..6).</summary>
    public sealed class Formation
    {
        readonly Tile[] slots;

        public Formation(string name, params Tile[] slots)
        {
            if (slots == null || slots.Length != Squad.Size) throw new ArgumentException("A formation places all 7 slots.", nameof(slots));
            Name = name;
            this.slots = slots;
        }

        public string Name { get; }

        public IReadOnlyList<Tile> Slots => slots;

        public Tile SlotTile(int slot, Side side) => side == Side.Home ? slots[slot] : Pitch.Mirror(slots[slot]);

        // Slots: K, A, B (anchors), P, Q (playmakers), R (runner), S (striker).
        // Outfield players stay at x <= 5 so the defending side is never touching the kick-off.
        public static readonly Formation TwoThreeOne = new Formation(
            "2-3-1",
            new Tile(0, 4),
            new Tile(2, 2), new Tile(2, 6),
            new Tile(4, 4), new Tile(4, 1),
            new Tile(4, 7),
            new Tile(5, 4));

        public static readonly Formation ThreeTwoOne = new Formation(
            "3-2-1",
            new Tile(0, 4),
            new Tile(2, 2), new Tile(2, 6),
            new Tile(2, 4), new Tile(4, 3),
            new Tile(4, 6),
            new Tile(5, 4));

        public static readonly Formation OneThreeTwo = new Formation(
            "1-3-2",
            new Tile(0, 4),
            new Tile(2, 4), new Tile(4, 4),
            new Tile(3, 1), new Tile(3, 7),
            new Tile(5, 2),
            new Tile(5, 6));

        public static IReadOnlyList<Formation> All { get; } = new[] { TwoThreeOne, ThreeTwoOne, OneThreeTwo };

        public static Formation ByName(string name)
        {
            foreach (var f in All)
                if (string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase)) return f;
            return null;
        }

        public override string ToString() => Name;
    }
}
