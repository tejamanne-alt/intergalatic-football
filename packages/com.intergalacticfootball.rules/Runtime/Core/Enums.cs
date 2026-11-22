namespace IntergalacticFootball.Rules
{
    /// <summary>Home defends the goal at x = -1 and attacks towards x = 15. Away does the opposite.</summary>
    public enum Side
    {
        Home = 0,
        Away = 1,
    }

    public enum Role
    {
        Keeper,
        Anchor,
        Playmaker,
        Runner,
        Striker,
    }

    /// <summary>A species' always-on rule.</summary>
    public enum Passive
    {
        None,

        /// <summary>
        /// Gravions, "can't be pushed off the ball": a carrier who loses a dribble contest is stopped
        /// but keeps the ball, and a carrier shoved by Shockwave keeps the ball.
        /// </summary>
        Anchored,

        /// <summary>Blinkers: a ball carrier ignores the first enemy zone they enter on each move.</summary>
        Phasing,
    }

    public enum PowerId
    {
        None,
        GravityWell,
        Shockwave,
        MeteorStrike,
        Blink,
        PhasePass,
        RiftTackle,
    }

    public enum UltimateId
    {
        None,
        Crush,
        FoldSpace,
    }

    public enum ActionType
    {
        Move,
        Pass,
        Shoot,
        Tackle,
        Power,

        /// <summary>The team ultimate. Needs full Flux, once per match, and costs no action.</summary>
        Ultimate,

        EndTurn,
    }

    public enum ContestKind
    {
        /// <summary>Ball carrier steps into an enemy zone: carrier Pace vs defender Tackle.</summary>
        Dribble,

        /// <summary>Tackle action: tackler Tackle vs carrier Pace.</summary>
        Tackle,

        /// <summary>Pass crosses an enemy zone: passer Pass vs defender Tackle.</summary>
        Interception,

        /// <summary>Shot crosses an outfield defender's zone: shooter Shot vs defender Tackle.</summary>
        Block,

        /// <summary>Keeper within reach of the shot: shooter Shot vs keeper Tackle.</summary>
        Save,

        /// <summary>No keeper within reach: a plain accuracy roll that gets harder with distance.</summary>
        Accuracy,
    }

    public static class SideExtensions
    {
        public static Side Opponent(this Side side) => side == Side.Home ? Side.Away : Side.Home;
    }
}
