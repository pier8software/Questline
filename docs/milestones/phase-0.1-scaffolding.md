# Phase 0.1: Project Scaffolding

## Status

Not Started

## Objective

Establish the project structure, world model, and basic navigation. Player can move between rooms and look around.

## Acceptance Criteria

### Project Structure
- [ ] Solution builds with `dotnet build`
- [ ] Tests run with `dotnet test`
- [ ] Namespace structure matches architecture spec (Cli, Domain, Engine, Framework)
- [ ] Dependency injection is configured
- [ ] .editorconfig is present with formatting rules

### World Model
- [ ] Rooms exist as entities with ID, name, and description
- [ ] Rooms have exits linking to other rooms by direction (north, south, east, west, up, down)
- [ ] World can be constructed programmatically (data loading comes in 0.3)

### Commands
- [ ] `look` — displays current room name, description, and available exits
- [ ] `go <direction>` — moves player to connected room if exit exists
- [ ] `go <direction>` — displays error if no exit in that direction
- [ ] Unknown commands display a helpful error message

### Game Loop
- [ ] Game starts and displays initial room
- [ ] Player can enter commands at a prompt
- [ ] `quit` exits the game gracefully

## Implementation Notes

### Suggested Order

1. Create solution and project structure
2. Define `Room` entity in Domain
3. Implement `World` as a room graph
4. Create parser skeleton that extracts verb and arguments
5. Implement `LookCommandHandler`
6. Implement `GoCommandHandler`
7. Wire up game loop in Cli
8. Add `quit` command

### Domain Entities

```csharp
// Suggested starting point
public class Room
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public Dictionary<Direction, string> Exits { get; init; } = new();
}

public enum Direction
{
    North, South, East, West, Up, Down
}
```

### Parser Design

Simple tokenisation for Phase 0:

```csharp
// "go north" → verb: "go", args: ["north"]
// "look" → verb: "look", args: []
```

Use a registration pattern for command handlers to enable extension without modifying dispatcher.

### Test Approach

Focus on handler behaviour:

```csharp
[Fact]
public void Look_DisplaysRoomDescription()
{
    // Arrange: room with description
    // Act: execute look command
    // Assert: result contains room description
}

[Fact]
public void GoNorth_WhenExitExists_MovesPlayer()
{
    // Arrange: two connected rooms
    // Act: execute go north
    // Assert: player location changed
}

[Fact]
public void GoNorth_WhenNoExit_ReturnsError()
{
    // Arrange: room with no north exit
    // Act: execute go north
    // Assert: error result, player unmoved
}
```

## Out of Scope

- Items
- NPCs
- Save/load
- Content loading from files
- Character creation
