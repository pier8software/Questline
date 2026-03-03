namespace Questline.Cli;

public static class RunModeParser
{
    private static readonly Dictionary<string, RunMode> ModeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["game"] = RunMode.Game,
        ["deploy-content"] = RunMode.DeployContent
    };

    public static RunMode Parse(string[] args)
    {
        var modeArg = args.FirstOrDefault(a => a.StartsWith("--mode=", StringComparison.OrdinalIgnoreCase));

        if (modeArg is null)
        {
            return RunMode.Game;
        }

        var value = modeArg["--mode=".Length..];

        if (ModeMap.TryGetValue(value, out var mode))
        {
            return mode;
        }

        var validModes = string.Join(", ", ModeMap.Keys);
        throw new ArgumentException($"Unknown mode '{value}'. Valid modes: {validModes}");
    }
}
