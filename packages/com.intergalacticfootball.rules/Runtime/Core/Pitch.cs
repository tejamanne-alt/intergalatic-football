using System;
using System.Collections.Generic;

namespace IntergalacticFootball.Rules
{
    /// <summary>
    /// The 15×9 pitch. Goals are 3 tiles wide (rows 3..5) and sit just outside the pitch:
    /// the home goal at x = -1, the away goal at x = 15.
    /// </summary>
    public static class Pitch
    {
        public const int Width = 15;
        public const int Height = 9;

        public const int GoalTop = 3;
        public const int GoalBottom = 5;
        public const int GoalCentreRow = 4;

        /// <summary>Penalty areas are 3 tiles deep and cover rows 1..7. Keepers can't leave theirs.</summary>
        public const int PenaltyDepth = 3;
        public const int PenaltyTop = 1;
        public const int PenaltyBottom = 7;

        public static readonly Tile Centre = new Tile(7, 4);

        static readonly Tile[] steps =
        {
            new Tile(-1, -1), new Tile(0, -1), new Tile(1, -1),
            new Tile(-1, 0), new Tile(1, 0),
            new Tile(-1, 1), new Tile(0, 1), new Tile(1, 1),
        };

        /// <summary>The eight unit offsets to neighbouring tiles.</summary>
        public static IReadOnlyList<Tile> Steps => steps;

        public static bool InBounds(Tile t) => t.X >= 0 && t.X < Width && t.Y >= 0 && t.Y < Height;

        public static bool IsGoalRow(int row) => row >= GoalTop && row <= GoalBottom;

        /// <summary>The off-pitch column holding the goal that <paramref name="defending"/> protects.</summary>
        public static int GoalColumn(Side defending) => defending == Side.Home ? -1 : Width;

        /// <summary>The goal tile itself (off the pitch) for a row of the goal that <paramref name="defending"/> protects.</summary>
        public static Tile GoalTile(Side defending, int row) => new Tile(GoalColumn(defending), row);

        /// <summary>The on-pitch tile right in front of the goal line.</summary>
        public static Tile GoalMouth(Side defending, int row) => new Tile(defending == Side.Home ? 0 : Width - 1, row);

        public static bool InPenaltyArea(Side owner, Tile t)
        {
            if (t.Y < PenaltyTop || t.Y > PenaltyBottom) return false;
            return owner == Side.Home
                ? t.X >= 0 && t.X < PenaltyDepth
                : t.X >= Width - PenaltyDepth && t.X < Width;
        }

        /// <summary>How far up the pitch a tile is for the attacking side: 0 at its own goal line, 14 at the opponent's.</summary>
        public static int Progress(Side attacking, Tile t) => attacking == Side.Home ? t.X : Width - 1 - t.X;

        /// <summary>Flips a home-orientation tile to the away half.</summary>
        public static Tile Mirror(Tile t) => new Tile(Width - 1 - t.X, t.Y);

        /// <summary>
        /// The tiles a ball travels through from <paramref name="from"/> to <paramref name="to"/>,
        /// excluding the start and including the end. Integer-only so every platform agrees.
        /// </summary>
        public static List<Tile> Line(Tile from, Tile to)
        {
            int dx = to.X - from.X;
            int dy = to.Y - from.Y;
            int n = Math.Max(Math.Abs(dx), Math.Abs(dy));
            var tiles = new List<Tile>(n);
            for (int i = 1; i <= n; i++)
                tiles.Add(new Tile(from.X + RoundDiv(dx * i, n), from.Y + RoundDiv(dy * i, n)));
            return tiles;
        }

        /// <summary>num / den rounded half away from zero, for den &gt; 0.</summary>
        static int RoundDiv(int num, int den) =>
            num >= 0 ? (2 * num + den) / (2 * den) : -((-2 * num + den) / (2 * den));
    }
}
