using System.Collections.Generic;

namespace IntergalacticFootball.Rules
{
    /// <summary>
    /// One action, as plain data. A match replay is the setup plus the list of actions;
    /// async PvP sends these to the server, which re-runs them through the same engine.
    /// </summary>
    public sealed class GameAction
    {
        static readonly Tile[] noPath = new Tile[0];

        GameAction(ActionType type, int playerId, Tile target, bool hasTarget, IReadOnlyList<Tile> path, int goalRow, int otherPlayerId)
        {
            Type = type;
            PlayerId = playerId;
            Target = target;
            HasTarget = hasTarget;
            Path = path ?? noPath;
            GoalRow = goalRow;
            OtherPlayerId = otherPlayerId;
        }

        public ActionType Type { get; }

        /// <summary>The acting player, or -1 for End Turn and Crush.</summary>
        public int PlayerId { get; }

        /// <summary>Pass target, Blink destination, Phase Pass target or Rift Tackle landing tile.</summary>
        public Tile Target { get; }

        public bool HasTarget { get; }

        /// <summary>Move path: each tile after the start, in order.</summary>
        public IReadOnlyList<Tile> Path { get; }

        /// <summary>Shot and Meteor Strike: goal row 3–5.</summary>
        public int GoalRow { get; }

        /// <summary>Second player for Fold Space.</summary>
        public int OtherPlayerId { get; }

        public static GameAction Move(int playerId, IReadOnlyList<Tile> path) =>
            new GameAction(ActionType.Move, playerId, default, false, path, 0, -1);

        public static GameAction Pass(int playerId, Tile target) =>
            new GameAction(ActionType.Pass, playerId, target, true, null, 0, -1);

        public static GameAction Shoot(int playerId, int goalRow = Pitch.GoalCentreRow) =>
            new GameAction(ActionType.Shoot, playerId, default, false, null, goalRow, -1);

        public static GameAction Tackle(int playerId) =>
            new GameAction(ActionType.Tackle, playerId, default, false, null, 0, -1);

        /// <summary>A power with no target (Gravity Well, Shockwave) or an automatic one (Rift Tackle).</summary>
        public static GameAction Power(int playerId) =>
            new GameAction(ActionType.Power, playerId, default, false, null, 0, -1);

        /// <summary>A power aimed at a tile (Blink, Phase Pass, Rift Tackle landing).</summary>
        public static GameAction Power(int playerId, Tile target) =>
            new GameAction(ActionType.Power, playerId, target, true, null, 0, -1);

        /// <summary>A power aimed at a goal row (Meteor Strike).</summary>
        public static GameAction PowerAtGoal(int playerId, int goalRow) =>
            new GameAction(ActionType.Power, playerId, default, false, null, goalRow, -1);

        /// <summary>An ultimate that needs no players (Crush).</summary>
        public static GameAction Ultimate() =>
            new GameAction(ActionType.Ultimate, -1, default, false, null, 0, -1);

        /// <summary>An ultimate on two of your players (Fold Space).</summary>
        public static GameAction Ultimate(int firstPlayerId, int secondPlayerId) =>
            new GameAction(ActionType.Ultimate, firstPlayerId, default, false, null, 0, secondPlayerId);

        public static GameAction EndTurn() =>
            new GameAction(ActionType.EndTurn, -1, default, false, null, 0, -1);

        public override string ToString()
        {
            switch (Type)
            {
                case ActionType.Move: return $"Move #{PlayerId} to {(Path.Count > 0 ? Path[Path.Count - 1].ToString() : "?")}";
                case ActionType.Pass: return $"Pass #{PlayerId} to {Target}";
                case ActionType.Shoot: return $"Shoot #{PlayerId} row {GoalRow}";
                case ActionType.Tackle: return $"Tackle #{PlayerId}";
                case ActionType.Power: return HasTarget ? $"Power #{PlayerId} at {Target}" : GoalRow != 0 ? $"Power #{PlayerId} row {GoalRow}" : $"Power #{PlayerId}";
                case ActionType.Ultimate: return PlayerId >= 0 ? $"Ultimate #{PlayerId}<->#{OtherPlayerId}" : "Ultimate";
                default: return "End turn";
            }
        }
    }
}
