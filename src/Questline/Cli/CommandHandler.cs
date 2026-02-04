namespace Questline.Cli;

public class CommandHandler
{
    // Temporary hardcoded room data for the base game loop
    private static readonly Dictionary<string, string> RoomDescriptions = new()
    {
        ["start"] =
            "You are standing in a small clearing. A path leads north into a dark forest. To the east, you can see the entrance to a cave.",
        ["forest"] =
            "You are in a dark forest. Tall trees surround you on all sides. The path continues south back to the clearing.",
        ["cave"] =
            "You are at the entrance of a cave. The darkness inside is impenetrable. The clearing is to the west."
    };

    private static readonly Dictionary<string, Dictionary<string, string>> RoomExits = new()
    {
        ["start"] = new Dictionary<string, string> { ["north"] = "forest", ["east"] = "cave" },
        ["forest"] = new Dictionary<string, string> { ["south"] = "start" },
        ["cave"] = new Dictionary<string, string> { ["west"] = "start" }
    };

    public CommandResult Execute(Command command, GameState state)
    {
        return command.Verb.ToLowerInvariant() switch
        {
            "look" or "l" => Look(state),
            "go" => Go(command.Noun, state),
            "inventory" or "i" => Inventory(state),
            "quit" or "q" => Quit(state),
            "help" => Help(state),
            "" => new CommandResult("I beg your pardon?", state),
            _ => new CommandResult($"I don't understand \"{command.Verb}\".", state)
        };
    }

    public string DescribeRoom(GameState state) =>
        RoomDescriptions.GetValueOrDefault(state.CurrentRoomId, "You are somewhere undefined.");

    private CommandResult Look(GameState state)
    {
        var description = DescribeRoom(state);
        return new CommandResult(description, state);
    }

    private CommandResult Go(string? direction, GameState state)
    {
        if (string.IsNullOrEmpty(direction))
        {
            return new CommandResult("Go where?", state);
        }

        var exits = RoomExits.GetValueOrDefault(state.CurrentRoomId);
        if (exits == null || !exits.TryGetValue(direction, out var destination))
        {
            return new CommandResult("You can't go that way.", state);
        }

        var newState = state
            .WithRoom(destination)
            .WithRoomVisited(destination);

        var description = DescribeRoom(newState);
        return new CommandResult(description, newState);
    }

    private CommandResult Inventory(GameState state)
    {
        if (state.Inventory.Count == 0)
        {
            return new CommandResult("You are empty-handed.", state);
        }

        var items = string.Join("\n  ", state.Inventory);
        return new CommandResult($"You are carrying:\n  {items}", state);
    }

    private CommandResult Quit(GameState state)
    {
        var newState = state.WithFlag("quit", true);
        return new CommandResult("Thanks for playing!", newState);
    }

    private CommandResult Help(GameState state)
    {
        const string helpText = """
                                Available commands:
                                  look (l)        - Look around the current room
                                  go <direction>  - Move in a direction (north, south, east, west)
                                  inventory (i)   - Check what you're carrying
                                  quit (q)        - Exit the game
                                  help            - Show this help message

                                You can also type directions directly (n, s, e, w).
                                """;
        return new CommandResult(helpText, state);
    }
}
