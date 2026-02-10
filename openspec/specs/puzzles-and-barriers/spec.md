# puzzles-and-barriers Specification

## Purpose

Define puzzle mechanics using the `use` command and barrier system. Players use items to unlock doors and overcome obstacles. The `examine` command reveals detailed information about items and room features.

## Requirements

### Requirement: Use command applies item in context

The `use <item>` command SHALL attempt to use an item from the player's inventory in the current context. The `use <item> on <target>` form SHALL use the item on a specific target.

#### Scenario: Use correct item on barrier

- **WHEN** the player has "rusty key" and executes `use rusty key on iron door` in a room with a locked "iron door" barrier
- **THEN** the barrier SHALL be unlocked and a success message SHALL be returned

#### Scenario: Use wrong item on barrier

- **WHEN** the player executes `use sword on iron door` and "sword" does not unlock "iron door"
- **THEN** an error message SHALL be returned and the barrier SHALL remain locked

#### Scenario: Use item not in inventory

- **WHEN** the player executes `use key` but has no "key" in inventory
- **THEN** an error result SHALL be returned

#### Scenario: Contextual use without target

- **WHEN** the player executes `use key` (no target) and the current room has exactly one barrier that accepts "key"
- **THEN** the barrier SHALL be unlocked

### Requirement: Barriers block exits

A barrier SHALL prevent movement through an exit until unlocked.

#### Scenario: Move through locked barrier

- **WHEN** the player executes `go north` and a locked barrier blocks the North exit
- **THEN** the player SHALL not move and a blocked message SHALL be returned

#### Scenario: Move through unlocked barrier

- **WHEN** the player executes `go north` and the North exit's barrier has been unlocked
- **THEN** the player SHALL move to the destination room

#### Scenario: Look shows barriers

- **WHEN** the player looks in a room with a locked barrier on the North exit
- **THEN** the look result SHALL describe the barrier (e.g. "A locked iron door blocks the way north")

### Requirement: Barrier state persists

Once a barrier is unlocked, it SHALL remain unlocked for the rest of the session.

#### Scenario: Barrier stays unlocked

- **WHEN** a barrier is unlocked and the player leaves and returns to the room
- **THEN** the barrier SHALL still be unlocked

### Requirement: Examine command shows detailed descriptions

The `examine <item>` command SHALL show the detailed description of an item. The `examine <feature>` command SHALL show detailed descriptions of room features.

#### Scenario: Examine item in inventory

- **WHEN** the player examines "rusty key" which is in their inventory
- **THEN** the result SHALL contain the item's detailed description

#### Scenario: Examine room feature

- **WHEN** the player examines "symbols" in a room with a "strange symbols" feature
- **THEN** the result SHALL contain the feature's detailed description

### Requirement: Five-room dungeon puzzle chain

The 5-room dungeon SHALL have a puzzle chain: the iron door in room 1 requires the rusty key, and using the key unlocks passage to room 2. The complete chain allows reaching room 5.

#### Scenario: Key unlocks iron door

- **WHEN** the player uses the rusty key on the iron door
- **THEN** the passage to the next room SHALL open

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

```csharp
public class Exit
{
    public required string Destination { get; init; }
    public string? BarrierId { get; init; }  // null = no barrier
}
```

### Room Features

Room features are examinable objects defined in content JSON:

```json
{
  "features": [
    {
      "id": "wall-symbols",
      "name": "strange symbols",
      "keywords": ["symbols", "carvings", "inscriptions"],
      "description": "The symbols appear to be an ancient script..."
    }
  ]
}
```

### Content: Barriers in JSON

```json
{
  "barriers": [
    {
      "id": "iron-door",
      "name": "iron door",
      "description": "A heavy iron door with a rusty lock.",
      "blockedMessage": "The iron door is locked tight.",
      "unlockItemId": "rusty-key",
      "unlockMessage": "The rusty key turns in the lock with a satisfying click."
    }
  ]
}
```
