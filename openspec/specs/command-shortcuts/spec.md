# command-shortcuts Specification

## Purpose

Define direction shortcuts and quality-of-life command aliases that make gameplay smoother. Includes single-letter direction commands, auto-look on room entry, and ensuring the full dungeon is playable end-to-end.

## Requirements

### Requirement: Direction shortcuts

The single-letter commands `n`, `s`, `e`, `w`, `u`, `d` SHALL be shortcuts for `go north`, `go south`, `go east`, `go west`, `go up`, `go down` respectively.

#### Scenario: North shortcut

- **WHEN** the player enters `n` and a North exit exists
- **THEN** the player SHALL move north (same as `go north`)

#### Scenario: All direction shortcuts

- **WHEN** the player enters `s`, `e`, `w`, `u`, or `d`
- **THEN** each SHALL behave as the corresponding `go <direction>` command

### Requirement: Auto-look on room entry

When the player moves to a new room, the game SHALL automatically display the room description (as if `look` were executed).

#### Scenario: Auto-look after movement

- **WHEN** the player moves to a new room via `go north` or `n`
- **THEN** the new room's name, description, exits, and items SHALL be displayed automatically

### Requirement: Commands are case-insensitive

All commands SHALL be case-insensitive (already specified in command-pipeline, reinforced here for completeness).

#### Scenario: Mixed case

- **WHEN** the player enters "Go North" or "GO NORTH"
- **THEN** it SHALL behave the same as "go north"

### Requirement: Extra whitespace ignored

Extra whitespace in commands SHALL be ignored (already specified in command-pipeline, reinforced here for completeness).

#### Scenario: Extra spaces

- **WHEN** the player enters "  go   north  "
- **THEN** it SHALL behave the same as "go north"

### Requirement: Complete dungeon playthrough

The 5-room dungeon SHALL be fully playable from start to finish using the implemented commands. All rooms SHALL have evocative descriptions, the puzzle SHALL be intuitive, and victory SHALL feel rewarding.

#### Scenario: End-to-end playthrough

- **WHEN** a player starts the game and uses only implemented commands
- **THEN** the player SHALL be able to reach the victory room and complete the dungeon

## Implementation Notes

### Direction Shortcut Handling

```csharp
// Register direction letters as aliases for the go command
// Input "n" -> GoCommand(Direction.North)
if (input is "n" or "s" or "e" or "w" or "u" or "d")
{
    var direction = ParseShortDirection(input);
    return new GoCommand(direction);
}
```

### Auto-Look in Game Loop

```csharp
if (result is MovedEvent moved)
{
    output.Write(moved.Narration);
    var lookResult = _lookHandler.Execute(state, new LookCommand());
    output.Write(lookResult);
}
```

### Playtesting Checklist

- Can complete dungeon from fresh start
- All commands work as documented
- No crashes on any input
- Victory screen displays correctly
