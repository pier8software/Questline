namespace Questline.Cli;

public class CommandParser
{
    private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = "north",
        ["s"] = "south",
        ["e"] = "east",
        ["w"] = "west",
        ["i"] = "inventory",
        ["l"] = "look",
        ["q"] = "quit",
        ["x"] = "examine"
    };

    private static readonly HashSet<string> DirectionVerbs = new(StringComparer.OrdinalIgnoreCase)
    {
        "north", "south", "east", "west", "up", "down",
        "n", "s", "e", "w", "u", "d"
    };

    public Command Parse(string input)
    {
        var trimmed = input.Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(trimmed))
        {
            return new Command("");
        }

        var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
        {
            return new Command("");
        }

        var firstWord = parts[0];

        // Handle direction shortcuts (typing "north" alone means "go north")
        if (DirectionVerbs.Contains(firstWord))
        {
            var direction = Aliases.GetValueOrDefault(firstWord, firstWord);
            return new Command("go", direction);
        }

        var verb = Aliases.GetValueOrDefault(firstWord, firstWord);

        if (parts.Length == 1)
        {
            return new Command(verb);
        }

        // Join remaining parts as the noun (handles "go north", "take brass lantern", etc.)
        var noun = string.Join(" ", parts.Skip(1));
        noun = Aliases.GetValueOrDefault(noun, noun);

        return new Command(verb, noun);
    }
}
