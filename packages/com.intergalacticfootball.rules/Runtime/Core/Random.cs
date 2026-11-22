namespace IntergalacticFootball.Rules
{
    /// <summary>
    /// PCG32. Used instead of System.Random because its output is defined by this code alone,
    /// so Mono, IL2CPP and a .NET server produce identical rolls for the same seed.
    /// </summary>
    public struct Pcg32
    {
        ulong state;
        readonly ulong increment;

        public Pcg32(ulong seed, ulong stream = 0xda3e39cb94b95bdbUL)
        {
            state = 0;
            increment = (stream << 1) | 1UL;
            NextUInt();
            state = unchecked(state + seed);
            NextUInt();
        }

        public uint NextUInt()
        {
            ulong old = state;
            state = unchecked(old * 6364136223846793005UL + increment);
            uint xorShifted = (uint)(((old >> 18) ^ old) >> 27);
            int rot = (int)(old >> 59);
            return (xorShifted >> rot) | (xorShifted << ((-rot) & 31));
        }

        /// <summary>Uniform integer in [0, bound).</summary>
        public int NextInt(int bound)
        {
            uint b = (uint)bound;
            uint threshold = unchecked(0u - b) % b;
            while (true)
            {
                uint r = NextUInt();
                if (r >= threshold) return (int)(r % b);
            }
        }

        /// <summary>Uniform double in [0, 1).</summary>
        public double NextDouble() => NextUInt() / 4294967296.0;
    }

    /// <summary>Supplies d100 rolls (0–99). A contest succeeds when roll &lt; chance.</summary>
    public interface IRollSource
    {
        int NextRoll();
    }

    /// <summary>
    /// Rolls that force each contest's outcome, in order. Chances are always clamped to 5–95,
    /// so a roll of 0 always succeeds and 99 always fails. Used by previews, the AI and tests.
    /// </summary>
    public sealed class ScriptedRolls : IRollSource
    {
        readonly bool[] script;
        readonly bool fallback;

        public ScriptedRolls(bool fallback, params bool[] script)
        {
            this.fallback = fallback;
            this.script = script ?? new bool[0];
        }

        public static ScriptedRolls AlwaysSucceed() => new ScriptedRolls(true);

        public static ScriptedRolls AlwaysFail() => new ScriptedRolls(false);

        /// <summary>How many rolls have been drawn.</summary>
        public int Used { get; private set; }

        public int NextRoll()
        {
            bool success = Used < script.Length ? script[Used] : fallback;
            Used++;
            return success ? 0 : 99;
        }
    }
}
