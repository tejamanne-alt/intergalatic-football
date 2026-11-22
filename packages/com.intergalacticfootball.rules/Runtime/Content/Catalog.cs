using System;
using System.Collections.Generic;

namespace IntergalacticFootball.Rules
{
    /// <summary>
    /// First-build content: two species (Gravions, Blinkers), their six powers and two ultimates.
    /// In Unity this becomes ScriptableObject assets that build the same definitions.
    /// </summary>
    public static class Catalog
    {
        // ---- Powers -------------------------------------------------------------------------

        public static readonly PowerDef GravityWell = new PowerDef(
            PowerId.GravityWell, "Gravity Well",
            "Pull the ball 2 tiles toward this player from up to 4 tiles away, even off an opponent's feet.",
            fluxCost: 4, cooldown: 3, range: 4, PowerTarget.None);

        public static readonly PowerDef Shockwave = new PowerDef(
            PowerId.Shockwave, "Shockwave",
            "Push every adjacent opponent 1 tile away. A pushed ball carrier drops the ball.",
            fluxCost: 3, cooldown: 2, range: 1, PowerTarget.None);

        public static readonly PowerDef MeteorStrike = new PowerDef(
            PowerId.MeteorStrike, "Meteor Strike",
            "A shot outfield players can't block. Only the keeper can stop it.",
            fluxCost: 4, cooldown: 3, range: 0, PowerTarget.GoalRow);

        public static readonly PowerDef Blink = new PowerDef(
            PowerId.Blink, "Blink",
            "Teleport up to 3 tiles, taking the ball along. No contests.",
            fluxCost: 3, cooldown: 3, range: 3, PowerTarget.Tile);

        public static readonly PowerDef PhasePass = new PowerDef(
            PowerId.PhasePass, "Phase Pass",
            "A pass that phases through every zone and can't be intercepted.",
            fluxCost: 3, cooldown: 2, range: 0, PowerTarget.PassTarget);

        public static readonly PowerDef RiftTackle = new PowerDef(
            PowerId.RiftTackle, "Rift Tackle",
            "Teleport next to an enemy ball carrier within 3 tiles and tackle at +10%.",
            fluxCost: 3, cooldown: 3, range: 3, PowerTarget.Tile);

        // ---- Ultimates ----------------------------------------------------------------------

        public static readonly UltimateDef Crush = new UltimateDef(
            UltimateId.Crush, "Crush",
            "Halve the opponents' Pace until the end of their next turn.",
            needsTwoPlayers: false);

        public static readonly UltimateDef FoldSpace = new UltimateDef(
            UltimateId.FoldSpace, "Fold Space",
            "Swap the positions of any two of your players.",
            needsTwoPlayers: true);

        // ---- Base stats by role (Pace, Pass, Shot, Tackle) ----------------------------------

        static readonly Stats[] baseStats =
        {
            new Stats(4, 5, 2, 8), // Keeper: Tackle is the save rating
            new Stats(4, 4, 3, 7), // Anchor
            new Stats(5, 8, 4, 5), // Playmaker
            new Stats(8, 5, 5, 4), // Runner
            new Stats(6, 4, 8, 3), // Striker
        };

        public static Stats BaseStats(Role role) => baseStats[(int)role];

        /// <summary>Base stats with a species modifier on every outfield role. Keepers stay neutral: a single
        /// point of keeper Tackle swings every save by 8%, which dwarfed everything else in sims.</summary>
        static Stats[] Modified(int pace, int pass, int shot, int tackle)
        {
            var result = new Stats[baseStats.Length];
            for (int i = 0; i < baseStats.Length; i++)
                result[i] = (Role)i == Role.Keeper ? baseStats[i] : baseStats[i].Add(pace, pass, shot, tackle);
            return result;
        }

        // ---- Species ------------------------------------------------------------------------

        public static readonly SpeciesDef Gravions = new SpeciesDef(
            "gravions", "Gravions", "Power",
            Passive.Anchored, "Anchored",
            "Can't be pushed off the ball: a carrier who loses a dribble contest keeps it, and Shockwave can't knock it loose.",
            Modified(pace: 0, pass: 0, shot: 1, tackle: 1),
            new[] { PowerId.GravityWell, PowerId.Shockwave, PowerId.MeteorStrike },
            new[]
            {
                PowerId.GravityWell,  // K
                PowerId.Shockwave,    // A
                PowerId.Shockwave,    // B
                PowerId.GravityWell,  // P
                PowerId.GravityWell,  // Q
                PowerId.Shockwave,    // R
                PowerId.MeteorStrike, // S
            },
            UltimateId.Crush);

        public static readonly SpeciesDef Blinkers = new SpeciesDef(
            "blinkers", "Blinkers", "Speed",
            Passive.Phasing, "Phasing",
            "A ball carrier ignores the first enemy zone they enter on each move.",
            Modified(pace: 1, pass: 0, shot: 0, tackle: -1),
            new[] { PowerId.Blink, PowerId.PhasePass, PowerId.RiftTackle },
            new[]
            {
                PowerId.Blink,      // K
                PowerId.RiftTackle, // A
                PowerId.RiftTackle, // B
                PowerId.PhasePass,  // P
                PowerId.PhasePass,  // Q
                PowerId.Blink,      // R
                PowerId.Blink,      // S
            },
            UltimateId.FoldSpace);

        public static IReadOnlyList<SpeciesDef> AllSpecies { get; } = new[] { Gravions, Blinkers };

        public static IReadOnlyList<PowerDef> AllPowers { get; } = new[] { GravityWell, Shockwave, MeteorStrike, Blink, PhasePass, RiftTackle };

        public static SpeciesDef SpeciesById(string id)
        {
            foreach (var s in AllSpecies)
                if (string.Equals(s.Id, id, StringComparison.OrdinalIgnoreCase) || string.Equals(s.Name, id, StringComparison.OrdinalIgnoreCase))
                    return s;
            return null;
        }

        public static PowerDef Power(PowerId id)
        {
            switch (id)
            {
                case PowerId.GravityWell: return GravityWell;
                case PowerId.Shockwave: return Shockwave;
                case PowerId.MeteorStrike: return MeteorStrike;
                case PowerId.Blink: return Blink;
                case PowerId.PhasePass: return PhasePass;
                case PowerId.RiftTackle: return RiftTackle;
                default: return null;
            }
        }

        public static UltimateDef Ultimate(UltimateId id)
        {
            switch (id)
            {
                case UltimateId.Crush: return Crush;
                case UltimateId.FoldSpace: return FoldSpace;
                default: return null;
            }
        }
    }
}
