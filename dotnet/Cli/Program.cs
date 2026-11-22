using System;
using IntergalacticFootball.Rules;

namespace IntergalacticFootball.Cli;

static class Program
{
    static int Main(string[] args)
    {
        Options o;
        try
        {
            o = Options.Parse(args);
        }
        catch (Exception e) when (e is ArgumentException || e is FormatException)
        {
            Console.Error.WriteLine(e.Message);
            Console.Error.WriteLine();
            Console.Error.WriteLine(Options.Usage);
            return 2;
        }

        switch (o.Mode)
        {
            case "play": return new Game(o, new[] { o.HumanSide }).Run();
            case "hotseat": return new Game(o, new[] { Side.Home, Side.Away }).Run();
            case "watch": return new Game(o, Array.Empty<Side>()).Run();
            case "sim": return Sim.Run(o);
            default:
                Console.WriteLine(Options.Usage);
                return o.Mode == "help" || o.Mode == "--help" || o.Mode == "-h" ? 0 : 2;
        }
    }
}
