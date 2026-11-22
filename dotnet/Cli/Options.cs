using System;
using IntergalacticFootball.Rules;
using IntergalacticFootball.Rules.Ai;

namespace IntergalacticFootball.Cli;

sealed class Options
{
    public string Mode = "help";
    public SpeciesDef Home = Catalog.Gravions;
    public SpeciesDef Away = Catalog.Blinkers;
    public Formation HomeFormation = Formation.TwoThreeOne;
    public Formation AwayFormation = Formation.TwoThreeOne;
    public AiDifficulty HomeAi = AiDifficulty.Medium;
    public AiDifficulty AwayAi = AiDifficulty.Medium;
    public Side HumanSide = Side.Home;
    public ulong? Seed;
    public int Matches = 20;
    public bool Color = !Console.IsOutputRedirected;
    public bool Confirm = true;
    public bool Swap = true;
    public int DelayMs;

    public const string Usage = """
        Intergalactic Football: terminal harness for the rules engine

        Usage: dotnet run --project dotnet/Cli -- <mode> [options]

        Modes
          play       You against the AI (you are Home unless --side away)
          hotseat    Pass-and-play: two humans share the terminal
          watch      AI against AI, printed move by move
          sim        AI against AI, many matches, prints pacing and balance stats

        Options
          --home <species>          gravions | blinkers        (default gravions)
          --away <species>                                      (default blinkers)
          --home-formation <f>      2-3-1 | 3-2-1 | 1-3-2      (default 2-3-1)
          --away-formation <f>
          --ai <level>              easy | medium | hard, both AIs (default medium)
          --home-ai / --away-ai     per-side AI level
          --side <home|away>        play: the side you control
          --seed <n>                match seed (sim: first seed)
          --matches <n>             sim: number of matches (default 20)
          --no-swap                 sim: keep species on the same side every match
          --delay <ms>              watch: pause between AI actions
          --no-confirm              play/hotseat: don't ask before risky actions
          --no-color                plain output
        """;

    public static Options Parse(string[] args)
    {
        var o = new Options();
        if (args.Length == 0) return o;
        o.Mode = args[0].ToLowerInvariant();
        for (int i = 1; i < args.Length; i++)
        {
            string a = args[i].ToLowerInvariant();
            string Next() => i + 1 < args.Length ? args[++i] : throw new ArgumentException($"{a} needs a value.");
            switch (a)
            {
                case "--home": o.Home = Species(Next()); break;
                case "--away": o.Away = Species(Next()); break;
                case "--home-formation": o.HomeFormation = Shape(Next()); break;
                case "--away-formation": o.AwayFormation = Shape(Next()); break;
                case "--ai": o.HomeAi = o.AwayAi = Level(Next()); break;
                case "--home-ai": o.HomeAi = Level(Next()); break;
                case "--away-ai": o.AwayAi = Level(Next()); break;
                case "--side": o.HumanSide = Next().ToLowerInvariant() == "away" ? Side.Away : Side.Home; break;
                case "--seed": o.Seed = ulong.Parse(Next()); break;
                case "--matches": o.Matches = int.Parse(Next()); break;
                case "--delay": o.DelayMs = int.Parse(Next()); break;
                case "--no-swap": o.Swap = false; break;
                case "--no-confirm": o.Confirm = false; break;
                case "--no-color": o.Color = false; break;
                default: throw new ArgumentException($"Unknown option {args[i]}.");
            }
        }
        return o;
    }

    static SpeciesDef Species(string id) => Catalog.SpeciesById(id) ?? throw new ArgumentException($"Unknown species {id}.");

    static Formation Shape(string name) => Formation.ByName(name) ?? throw new ArgumentException($"Unknown formation {name}.");

    static AiDifficulty Level(string name) => Enum.TryParse<AiDifficulty>(name, true, out var d) ? d : throw new ArgumentException($"Unknown AI level {name}.");
}
