# Phase 0.7: Polish

## Status

Not Started

## Objective

Polish the player experience with better help, robust error handling, and end-game detection. The 5-room dungeon should feel complete and satisfying to play through.

## Acceptance Criteria

### Help System
- [ ] `help` displays list of available commands with brief descriptions
- [ ] `help <command>` displays detailed help for specific command
- [ ] Unknown command suggests "Type 'help' for available commands"

### Error Handling
- [ ] All commands handle edge cases gracefully
- [ ] No unhandled exceptions crash the game
- [ ] Errors display in consistent, friendly format
- [ ] Stack traces only in debug mode

### Unknown Input
- [ ] Unrecognised commands produce varied, flavourful responses
- [ ] Suggestions offered when input is close to valid command
- [ ] "Did you mean...?" for typos

### End Game
- [ ] Victory condition detected when player reaches treasure room
- [ ] Victory message displayed with summary
- [ ] Option to play again or quit
- [ ] (Optional) Track completion time

### Quality of Life
- [ ] Command shortcuts work (`n` for `go north`, `i` for `inventory`, `l` for `look`)
- [ ] Commands are case-insensitive
- [ ] Extra whitespace is ignored
- [ ] `look` automatically triggered when entering new room

### The Complete Experience
- [ ] 5-room dungeon is fully playable from start to finish
- [ ] All rooms have evocative descriptions
- [ ] Puzzle is intuitive but satisfying
- [ ] Victory feels rewarding

## Implementation Notes

### Help System

```csharp
public class HelpCommandHandler : ICommandHandler
{
    private readonly ICommandRegistry _registry;
    
    public CommandResult Execute(GameState state, HelpCommand command)
    {
        if (command.Topic is null)
            return ShowAllCommands();
        
        return ShowCommandHelp(command.Topic);
    }
}
```

Command metadata:

```csharp
public record CommandInfo(
    string Name,
    string[] Aliases,
    string Brief,          // "Move in a direction"
    string Detailed,       // "Usage: go <direction>\n\nMoves your character..."
    string[] Examples      // ["go north", "go n", "n"]
);
```

Help output:

```
> help

Available commands:
  look (l)      - Examine your surroundings
  go (n/s/e/w)  - Move in a direction
  get           - Pick up an item
  drop          - Drop an item
  inventory (i) - View carried items
  use           - Use an item
  examine (x)   - Examine something closely
  stats         - View character stats
  save          - Save your game
  help          - Show this help
  quit          - Exit the game

Type 'help <command>' for detailed information.
```

### Varied Unknown Command Responses

Rotate through responses to keep personality:

```csharp
private static readonly string[] UnknownResponses = [
    "I don't understand '{0}'.",
    "That's not something you can do.",
    "'{0}'? I'm not sure what you mean.",
    "You ponder '{0}' but nothing comes of it.",
    "That doesn't make sense here."
];
```

### Typo Detection

Simple Levenshtein distance for suggestions:

```csharp
public string? SuggestCommand(string input)
{
    var verbs = _registry.GetAllVerbs();
    var closest = verbs
        .Select(v => (verb: v, distance: LevenshteinDistance(input, v)))
        .Where(x => x.distance <= 2)
        .OrderBy(x => x.distance)
        .FirstOrDefault();
    
    return closest.verb;
}

// "hlep" → "Did you mean 'help'?"
```

### Command Shortcuts

Configure in command registry:

```csharp
services.AddCommandHandler<GoCommandHandler>("go", "walk", "move", "n", "s", "e", "w", "u", "d");
services.AddCommandHandler<LookCommandHandler>("look", "l", "examine", "x");
services.AddCommandHandler<InventoryCommandHandler>("inventory", "inv", "i");
```

Handle directional shortcuts:

```csharp
// Input "n" → GoCommand(Direction.North)
if (input is "n" or "s" or "e" or "w" or "u" or "d")
{
    var direction = ParseShortDirection(input);
    return new GoCommand(direction);
}
```

### Victory Detection

Tag rooms with victory condition:

```json
{
  "id": "treasure-room",
  "name": "The Treasure Chamber",
  "description": "Mountains of gold glitter in the torchlight...",
  "isVictory": true
}
```

On room entry:

```csharp
if (room.IsVictory)
{
    return new VictoryEvent(CalculateStats());
}
```

Victory display:

```
═══════════════════════════════════════════════════
                    VICTORY!
═══════════════════════════════════════════════════

Congratulations, Thorin! You have conquered the 
Goblin's Lair and claimed the treasure!

Rooms explored: 5/5
Items collected: 3
Time: 12 minutes

═══════════════════════════════════════════════════

Play again? (y/n)
```

### Automatic Look on Entry

In game loop:

```csharp
if (result is MovedEvent moved)
{
    // Display movement narration
    output.Write(moved.Narration);
    
    // Auto-look at new room
    var lookResult = _lookHandler.Execute(state, new LookCommand());
    output.Write(lookResult);
}
```

### Error Display

Consistent formatting:

```csharp
public void WriteError(string message)
{
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine(message);
    Console.ResetColor();
}
```

No stack traces in release:

```csharp
catch (Exception ex)
{
    #if DEBUG
    Console.WriteLine(ex.ToString());
    #else
    Console.WriteLine("Something went wrong. Please try again.");
    Logger.Error(ex);
    #endif
}
```

### Test Approach

```csharp
[Fact]
public void Help_NoTopic_ListsAllCommands()
{
    // Act
    var result = handler.Execute(state, new HelpCommand());
    
    // Assert
    result.Text.ShouldContain("go");
    result.Text.ShouldContain("look");
    result.Text.ShouldContain("inventory");
}

[Fact]
public void UnknownCommand_SuggestsSimilar()
{
    // Arrange: input "hlep"
    // Act
    var result = parser.Parse("hlep");
    
    // Assert
    result.Suggestion.ShouldBe("help");
}

[Fact]
public void EnterVictoryRoom_TriggersVictory()
{
    // Arrange: player adjacent to victory room
    // Act: move to victory room
    // Assert: VictoryEvent returned
}

[Fact]
public void Shortcut_N_MovesNorth()
{
    // Arrange: room with north exit
    // Act: execute "n"
    // Assert: player moved north
}
```

### Playtesting Checklist

Before marking Phase 0 complete, verify:

- [ ] Can complete dungeon from fresh start
- [ ] Can save mid-adventure and resume
- [ ] All commands work as documented
- [ ] No crashes on any input
- [ ] Victory screen displays correctly
- [ ] Time to complete: 5-10 minutes (appropriate for demo)

## Out of Scope

- Sound effects
- Colour themes/customisation
- Achievements
- Leaderboards
- Difficulty levels
