# help-system Specification

## Purpose

Define the in-game help system. Players can view a list of all available commands or get detailed help for a specific command. Unknown commands suggest using help.

## Requirements

### Requirement: Help lists all commands

The `help` command with no arguments SHALL display a list of all available commands with brief descriptions.

#### Scenario: Help with no arguments

- **WHEN** the player executes `help`
- **THEN** the result SHALL list all registered commands with their aliases and brief descriptions

### Requirement: Help shows detailed command information

The `help <command>` command SHALL display detailed help for the specified command, including usage, description, and examples.

#### Scenario: Help for specific command

- **WHEN** the player executes `help go`
- **THEN** the result SHALL contain the command's detailed description, usage syntax, and examples

#### Scenario: Help for unknown command

- **WHEN** the player executes `help fly` and "fly" is not a registered command
- **THEN** the result SHALL indicate the command is not recognised

### Requirement: Unknown command suggests help

When an unrecognised command is entered, the error message SHALL suggest "Type 'help' for available commands."

#### Scenario: Unknown command hint

- **WHEN** the player enters an unrecognised command
- **THEN** the error result SHALL include a suggestion to type 'help'

### Requirement: CommandInfo metadata

Each registered command SHALL have metadata including name, aliases, brief description, detailed description, and examples.

#### Scenario: Command metadata

- **WHEN** the "go" command is registered
- **THEN** its CommandInfo SHALL include name "go", aliases, brief "Move in a direction", and examples

## Implementation Notes

### CommandInfo Record

```csharp
public record CommandInfo(
    string Name,
    string[] Aliases,
    string Brief,       // "Move in a direction"
    string Detailed,    // "Usage: go <direction>\n\nMoves your character..."
    string[] Examples   // ["go north", "n"]
);
```

### Help Output Format

```
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
