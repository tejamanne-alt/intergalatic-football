using System;

namespace IntergalacticFootball.Rules
{
    /// <summary>
    /// Hand-builds match situations for puzzles ("score in 2 turns"), tutorials and tests.
    /// Not for use during normal play: it bypasses the rules.
    /// </summary>
    public static class Scenario
    {
        /// <summary>Puts a player on a tile. If the loose ball is there, they pick it up.</summary>
        public static void Place(MatchState s, int playerId, Tile tile)
        {
            var p = s.GetPlayer(playerId);
            if (!s.CanStandOn(p, tile)) throw new ArgumentException($"{p.Label} can't stand on {tile}.");
            var other = s.PlayerAt(tile);
            if (other != null && other != p) throw new ArgumentException($"{tile} is taken by {other.Label}.");
            p.Pos = tile;
            if (s.BallIsLoose && s.LooseBallTile == tile) s.GiveBall(p);
        }

        /// <summary>Takes a player off the pitch. Benched players have no zone and can't act.</summary>
        public static void Bench(MatchState s, int playerId)
        {
            if (s.BallCarrierId == playerId) s.DropBall(s.GetPlayer(playerId).Pos);
            s.GetPlayer(playerId).Pos = Tile.Bench;
        }

        /// <summary>Benches all 14 players and leaves the ball loose at the centre spot.</summary>
        public static void ClearPitch(MatchState s)
        {
            s.DropBall(Pitch.Centre);
            foreach (var p in s.Players) p.Pos = Tile.Bench;
        }

        public static void GiveBall(MatchState s, int playerId)
        {
            var p = s.GetPlayer(playerId);
            if (!s.IsOnPitch(p)) throw new ArgumentException($"{p.Label} is not on the pitch.");
            s.GiveBall(p);
        }

        /// <summary>Puts the ball on a tile; whoever stands there gets it.</summary>
        public static void PlaceBall(MatchState s, Tile tile)
        {
            if (!Pitch.InBounds(tile)) throw new ArgumentException($"{tile} is off the pitch.");
            var p = s.PlayerAt(tile);
            if (p != null) s.GiveBall(p);
            else s.DropBall(tile);
        }

        public static void SetFlux(MatchState s, Side side, int flux) =>
            s.Team(side).Flux = Math.Max(0, Math.Min(s.Config.MaxFlux, flux));

        public static void SetScore(MatchState s, int home, int away)
        {
            s.Team(Side.Home).Score = home;
            s.Team(Side.Away).Score = away;
        }

        /// <summary>Jumps to a turn. The side to act follows from the turn index (Home on even turns in the first half).</summary>
        public static void SetTurn(MatchState s, int turnIndex, int? actionsLeft = null)
        {
            s.TurnIndex = turnIndex;
            s.ActiveSide = s.SideForTurn(turnIndex);
            s.ActionsLeft = actionsLeft ?? s.Config.ActionsPerTurn;
            s.IsOver = false;
        }

        public static void SetCooldown(MatchState s, int playerId, int turns) => s.GetPlayer(playerId).PowerCooldown = turns;

        public static void MarkUltimateUsed(MatchState s, Side side) => s.Team(side).UltimateUsed = true;

        /// <summary>Re-seeds the match's roll stream.</summary>
        public static void Reseed(MatchState s, ulong seed) => s.Rng = new Pcg32(seed);
    }
}
