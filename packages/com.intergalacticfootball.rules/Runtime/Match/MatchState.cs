using System;
using System.Collections.Generic;

namespace IntergalacticFootball.Rules
{
    /// <summary>
    /// Everything about a match in progress. Small and cheap to clone, so the AI can search over copies.
    /// Mutate it only through <see cref="MatchEngine.Apply"/> (or <see cref="Scenario"/> when building puzzles and tests).
    /// </summary>
    public sealed class MatchState
    {
        readonly Player[] players;
        readonly TeamState[] teams;

        /// <summary>The match's own roll stream; part of the state so replays and server checks reproduce it.</summary>
        internal Pcg32 Rng;

        MatchState(MatchSetup setup, Player[] players, TeamState[] teams, Pcg32 rng)
        {
            Setup = setup;
            this.players = players;
            this.teams = teams;
            Rng = rng;
            BallCarrierId = -1;
            LooseBallTile = Tile.Bench;
        }

        public MatchSetup Setup { get; }
        public RulesConfig Config => Setup.Config;
        public IReadOnlyList<Player> Players => players;

        /// <summary>Id of the player with the ball, or -1 when the ball is loose.</summary>
        public int BallCarrierId { get; private set; }

        /// <summary>Where the ball lies when nobody has it.</summary>
        public Tile LooseBallTile { get; private set; }

        public Side ActiveSide { get; internal set; }
        public int ActionsLeft { get; internal set; }

        /// <summary>Team-turns played so far, 0 to <see cref="RulesConfig.TotalTurns"/>.</summary>
        public int TurnIndex { get; internal set; }

        public bool IsOver { get; internal set; }

        public static MatchState Create(MatchSetup setup)
        {
            if (setup == null) throw new ArgumentNullException(nameof(setup));
            var players = new Player[Squad.Size * 2];
            var teams = new TeamState[2];
            for (int sideIndex = 0; sideIndex < 2; sideIndex++)
            {
                var side = (Side)sideIndex;
                var team = setup.For(side);
                teams[sideIndex] = new TeamState(side, team);
                for (int slot = 0; slot < Squad.Size; slot++)
                {
                    var role = Squad.RoleOf(slot);
                    int id = sideIndex * Squad.Size + slot;
                    players[id] = new Player(id, side, slot, role, Squad.LabelOf(slot, side), team.Species.StatsFor(role), team.Loadout[slot]);
                }
            }

            var state = new MatchState(setup, players, teams, new Pcg32(setup.Seed));
            state.ResetForKickOff(Side.Home);
            state.TurnIndex = 0;
            state.BeginTurn();
            return state;
        }

        // ---- Queries ------------------------------------------------------------------------

        public Player GetPlayer(int id) => players[id];

        public TeamState Team(Side side) => teams[(int)side];

        public ArraySegment<Player> PlayersOf(Side side) => new ArraySegment<Player>(players, (int)side * Squad.Size, Squad.Size);

        public Player Keeper(Side side) => players[(int)side * Squad.Size + Squad.KeeperSlot];

        public Player Carrier => BallCarrierId >= 0 ? players[BallCarrierId] : null;

        public bool BallIsLoose => BallCarrierId < 0;

        public Tile BallTile => BallCarrierId >= 0 ? players[BallCarrierId].Pos : LooseBallTile;

        public Player PlayerAt(Tile tile)
        {
            if (!Pitch.InBounds(tile)) return null;
            for (int i = 0; i < players.Length; i++)
                if (players[i].Pos == tile) return players[i];
            return null;
        }

        public bool IsOnPitch(Player p) => Pitch.InBounds(p.Pos);

        /// <summary>In bounds, and inside the penalty area for keepers. Ignores who else is there.</summary>
        public bool CanStandOn(Player p, Tile t) =>
            Pitch.InBounds(t) && (p.Role != Role.Keeper || Pitch.InPenaltyArea(p.Side, t));

        public int Half => TurnIndex < Config.TurnsPerHalf ? 1 : 2;

        /// <summary>1-based turn number of the active side within the current half (1–10).</summary>
        public int TurnOfHalf => (Math.Min(TurnIndex, Config.TotalTurns - 1) % Config.TurnsPerHalf) / 2 + 1;

        /// <summary>Home kicks off and acts first in the first half, Away in the second.</summary>
        public Side SideForTurn(int turnIndex)
        {
            var kickOff = turnIndex < Config.TurnsPerHalf ? Side.Home : Side.Away;
            return (turnIndex % Config.TurnsPerHalf) % 2 == 0 ? kickOff : kickOff.Opponent();
        }

        /// <summary>The first turn index after <paramref name="afterTurn"/> that belongs to <paramref name="side"/>.</summary>
        public int NextTurnOf(Side side, int afterTurn)
        {
            for (int t = afterTurn + 1; t < Config.TotalTurns; t++)
                if (SideForTurn(t) == side) return t;
            return Config.TotalTurns;
        }

        public int EffectivePace(Player p)
        {
            int pace = p.Stats.Pace;
            if (TurnIndex <= Team(p.Side).PaceHalvedThroughTurn) pace = Math.Max(1, pace / 2);
            return pace;
        }

        public int MoveRange(Player p) => Config.MoveRange(EffectivePace(p));

        public int PassRange(Player p) => Config.PassRange(p.Stats.Pass);

        public int ShotRange(Player p) => Config.ShotRange(p.Stats.Shot);

        public bool UltimateReady(Side side)
        {
            var team = Team(side);
            return team.Species.Ultimate != UltimateId.None && !team.UltimateUsed && team.Flux >= Config.MaxFlux;
        }

        public MatchState Clone()
        {
            var ps = new Player[players.Length];
            for (int i = 0; i < ps.Length; i++) ps[i] = players[i].Clone();
            var ts = new[] { teams[0].Clone(), teams[1].Clone() };
            return new MatchState(Setup, ps, ts, Rng)
            {
                BallCarrierId = BallCarrierId,
                LooseBallTile = LooseBallTile,
                ActiveSide = ActiveSide,
                ActionsLeft = ActionsLeft,
                TurnIndex = TurnIndex,
                IsOver = IsOver,
            };
        }

        // ---- Mutation (rules engine only) ---------------------------------------------------

        internal void GiveBall(Player p)
        {
            BallCarrierId = p.Id;
            LooseBallTile = Tile.Bench;
        }

        internal void DropBall(Tile t)
        {
            BallCarrierId = -1;
            LooseBallTile = t;
        }

        internal int NextRoll() => Rng.NextInt(100);

        /// <summary>Adds Flux up to the cap and returns how much was actually gained.</summary>
        internal int AddFlux(Side side, int amount)
        {
            var team = Team(side);
            int before = team.Flux;
            team.Flux = Math.Min(Config.MaxFlux, team.Flux + amount);
            return team.Flux - before;
        }

        internal void ResetForKickOff(Side kicking)
        {
            foreach (var p in players)
                p.Pos = Team(p.Side).Formation.SlotTile(p.Slot, p.Side);
            var striker = players[(int)kicking * Squad.Size + Squad.StrikerSlot];
            striker.Pos = Pitch.Centre;
            GiveBall(striker);
        }

        internal void BeginTurn()
        {
            ActiveSide = SideForTurn(TurnIndex);
            ActionsLeft = Config.ActionsPerTurn;
            foreach (var p in PlayersOf(ActiveSide))
                if (p.PowerCooldown > 0) p.PowerCooldown--;
        }
    }
}
