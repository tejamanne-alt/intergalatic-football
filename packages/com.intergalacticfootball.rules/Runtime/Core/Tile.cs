using System;
using System.Globalization;

namespace IntergalacticFootball.Rules
{
    /// <summary>
    /// A tile on the pitch grid. X runs 0..14 from the home goal to the away goal, Y runs 0..8.
    /// Distances are Chebyshev (diagonal steps count as one), so "within 1" means the 8 surrounding tiles.
    /// </summary>
    public readonly struct Tile : IEquatable<Tile>
    {
        /// <summary>Where players who are not on the pitch (puzzle setups) are kept. Out of bounds, far from everything.</summary>
        public static readonly Tile Bench = new Tile(-100, -100);

        public readonly int X;
        public readonly int Y;

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static int Distance(Tile a, Tile b) => Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));

        public int DistanceTo(Tile other) => Distance(this, other);

        public Tile Offset(int dx, int dy) => new Tile(X + dx, Y + dy);

        public bool Equals(Tile other) => X == other.X && Y == other.Y;

        public override bool Equals(object obj) => obj is Tile other && Equals(other);

        public override int GetHashCode() => (X * 397) ^ Y;

        public static bool operator ==(Tile a, Tile b) => a.Equals(b);

        public static bool operator !=(Tile a, Tile b) => !a.Equals(b);

        public override string ToString() => X.ToString(CultureInfo.InvariantCulture) + "," + Y.ToString(CultureInfo.InvariantCulture);

        /// <summary>Parses "x,y".</summary>
        public static bool TryParse(string text, out Tile tile)
        {
            tile = default;
            if (string.IsNullOrWhiteSpace(text)) return false;
            var parts = text.Split(',');
            if (parts.Length != 2) return false;
            if (!int.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var x)) return false;
            if (!int.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var y)) return false;
            tile = new Tile(x, y);
            return true;
        }
    }
}
