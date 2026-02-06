# Phase 0.4: Puzzles and Barriers

## Status

Not Started

## Objective

Implement the `use` command and barrier mechanics. Players can use items to unlock doors and overcome obstacles, enabling the puzzle elements of the 5-room dungeon.

## Acceptance Criteria

### Use Command
- [ ] `use <item>` — attempts to use item in current context
- [ ] `use <item> on <target>` — uses item on specific target
- [ ] Using correct item on barrier unlocks/removes it
- [ ] Using wrong item produces appropriate feedback
- [ ] Using item not in inventory produces error

### Barriers
- [ ] Barriers can block exits (locked doors, collapsed rubble, etc.)
- [ ] `look` shows barriers: "A locked iron door blocks the way north"
- [ ] Attempting to move through barrier produces feedback
- [ ] Unlocked barriers allow passage
- [ ] Barrier state persists (unlocked stays unlocked)

### Examine Command
- [ ] `examine <item>` — shows detailed item description
- [ ] `examine <feature>` — examines room features (doors, inscriptions, etc.)
- [ ] Examining can reveal hidden information or items

### The 5-Room Dungeon Puzzle
- [ ] Iron door in room 1 requires rusty key
- [ ] Using key on door unlocks passage to room 2
- [ ] Complete puzzle chain allows reaching room 5

## Implementation Notes

### Barrier Model

```csharp
public class Barrier
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string BlockedMessage { get; init; }
    public required string UnlockItemId { get; init; }
    public required string UnlockMessage { get; init; }
    public bool IsUnlocked { get; set; }
}
```

### Exit with Barrier

Extend exit model:

```csharp
public class Exit
{
    public required string Destination { get; init; }
    public string? BarrierId { get; init; }  // null = no barrier
}
```

### Content Definition

```json
{
  "barriers": [
    {
      "id": "iron-door",
      "name": "iron door",
      "description": "A heavy iron door with a rusty lock.",
      "blockedMessage": "The iron door is locked tight.",
      "unlockItemId": "rusty-key",
      "unlockMessage": "The rusty key turns in the lock with a satisfying click. The door swings open."
    }
  ]
}
```

### Use Command Handler

```csharp
public class UseCommandHandler : ICommandHandler
{
    public CommandResult Execute(GameState state, UseCommand command)
    {
        var item = state.Player.Inventory.Find(command.ItemName);
        if (item is null)
            return CommandResult.Error($"You don't have a {command.ItemName}.");

        // If target specified, try to use on target
        if (command.Target is not null)
        {
            return TryUseOn(state, item, command.Target);
        }

        // Otherwise, try contextual use (barriers in current room)
        return TryContextualUse(state, item);
    }
}
```

### Contextual Use

If player types `use key` without a target:

1. Check current room for barriers that accept this item
2. If exactly one match, use it
3. If multiple matches, ask for clarification
4. If no matches, "You can't figure out how to use that here."

### Room Features

Add examinable features to rooms for richer exploration:

```json
{
  "id": "puzzle-room",
  "name": "The Locked Chamber",
  "description": "A heavy iron door blocks the way forward.",
  "features": [
    {
      "id": "wall-symbols",
      "name": "strange symbols",
      "keywords": ["symbols", "carvings", "inscriptions", "walls"],
      "description": "The symbols appear to be an ancient script. One phrase is repeated: 'The key lies where light meets dark.'"
    }
  ]
}
```

### Test Approach

```csharp
[Fact]
public void Use_CorrectItemOnBarrier_UnlocksBarrier()
{
    // Arrange: player with key, room with locked door
    // Act: "use key on door"
    // Assert: barrier unlocked, success message
}

[Fact]
public void Use_WrongItem_ProducesHint()
{
    // Arrange: player with wrong item, locked door
    // Act: "use sword on door"
    // Assert: error message, barrier still locked
}

[Fact]
public void Go_ThroughUnlockedBarrier_Succeeds()
{
    // Arrange: unlocked barrier
    // Act: "go north"
    // Assert: player moves to destination
}

[Fact]
public void Go_ThroughLockedBarrier_Blocked()
{
    // Arrange: locked barrier
    // Act: "go north"
    // Assert: blocked message, player unmoved
}

[Fact]
public void Examine_Feature_RevealsDescription()
{
    // Arrange: room with examinable symbols
    // Act: "examine symbols"
    // Assert: detailed description shown
}
```

## Out of Scope

- Complex multi-step puzzles
- Consumable items (key is not consumed)
- Combination puzzles
- Timed puzzles
- Combat-based challenges
