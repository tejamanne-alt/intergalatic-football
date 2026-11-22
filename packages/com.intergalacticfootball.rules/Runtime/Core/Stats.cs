using System;

namespace IntergalacticFootball.Rules
{
    /// <summary>The four player stats, each on a 1–10 scale.</summary>
    public readonly struct Stats
    {
        public const int Min = 1;
        public const int Max = 10;

        public readonly int Pace;
        public readonly int Pass;
        public readonly int Shot;

        /// <summary>The defensive stat. A keeper's Tackle is their save rating.</summary>
        public readonly int Tackle;

        public Stats(int pace, int pass, int shot, int tackle)
        {
            Pace = Clamp(pace);
            Pass = Clamp(pass);
            Shot = Clamp(shot);
            Tackle = Clamp(tackle);
        }

        public Stats Add(int pace, int pass, int shot, int tackle) =>
            new Stats(Pace + pace, Pass + pass, Shot + shot, Tackle + tackle);

        static int Clamp(int v) => Math.Max(Min, Math.Min(Max, v));

        public override string ToString() => $"Pace {Pace}, Pass {Pass}, Shot {Shot}, Tackle {Tackle}";
    }
}
