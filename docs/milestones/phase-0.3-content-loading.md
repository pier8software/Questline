# Phase 0.3: Content Loading

## Status

Complete

## Objective

Define the 5-room dungeon adventure as JSON data files and load them at runtime. Decouple content authoring from code.

## Acceptance Criteria

### Content Structure
- [x] Adventure content lives in `content/adventures/five-room-dungeon/`
- [x] Rooms defined in JSON with connections, descriptions, and items
- [x] Items defined in JSON with properties
- [x] Adventure metadata (name, starting room) defined in manifest

### Content Loading
- [x] Game loads adventure from content files on startup
- [x] World is constructed from loaded data
- [x] Invalid content produces clear error messages
- [x] Missing required fields fail fast with helpful errors

### The 5-Room Dungeon
- [x] Room 1: Entrance with a challenge/guardian (puzzle in 0.4)
- [x] Room 2: Puzzle or roleplaying challenge
- [x] Room 3: Trick or setback
- [x] Room 4: Climax / big challenge
- [x] Room 5: Reward / revelation

## Implementation Notes

### Content File Structure

```
content/
└── adventures/
    └── five-room-dungeon/
        ├── adventure.json      # Manifest
        ├── rooms.json          # Room definitions
        └── items.json          # Item definitions
```

### adventure.json

```json
{
  "id": "five-room-dungeon",
  "name": "The Goblin's Lair",
  "description": "A short adventure to test your mettle.",
  "startingRoomId": "entrance",
  "version": "1.0"
}
```

### rooms.json

```json
{
  "rooms": [
    {
      "id": "entrance",
      "name": "Cave Entrance",
      "description": "Daylight filters into a narrow cave mouth. The smell of damp earth fills your nostrils.",
      "exits": {
        "north": "puzzle-room"
      },
      "items": ["rusty-key"]
    },
    {
      "id": "puzzle-room",
      "name": "The Locked Chamber",
      "description": "A heavy iron door blocks the way forward. Strange symbols are carved into the stone walls.",
      "exits": {
        "south": "entrance",
        "north": {
          "destination": "setback-room",
          "barrier": "iron-door"
        }
      }
    }
    // ... more rooms
  ]
}
```

### items.json

```json
{
  "items": [
    {
      "id": "rusty-key",
      "name": "rusty key",
      "description": "An old iron key, its teeth worn by time."
    },
    {
      "id": "gold-chalice",
      "name": "gold chalice",
      "description": "A gleaming chalice encrusted with rubies."
    }
  ]
}
```

### Serialisation

Use System.Text.Json with options:

```csharp
var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true
};
```

### Content Loader

```csharp
public interface IAdventureLoader
{
    Adventure Load(string adventurePath);
}

public class Adventure
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required World World { get; init; }
    public required string StartingRoomId { get; init; }
}
```

### Validation

On load, validate:
- All room exits reference valid room IDs
- All room item references exist in items.json
- Starting room exists
- No orphaned rooms (unreachable from start)

### Test Approach

```csharp
[Fact]
public void Load_ValidAdventure_ConstructsWorld()
{
    // Arrange: valid content files
    // Act: load adventure
    // Assert: world contains expected rooms and items
}

[Fact]
public void Load_MissingRoom_ThrowsWithDetails()
{
    // Arrange: exit references non-existent room
    // Act/Assert: throws with clear error message
}

[Fact]
public void Load_InvalidJson_ThrowsWithDetails()
{
    // Arrange: malformed JSON
    // Act/Assert: throws with file name and error location
}
```

## The Five-Room Dungeon Design

Using the classic 5-room dungeon structure:

| Room | Purpose | Implementation |
|------|---------|----------------|
| 1. Entrance | Guardian/challenge | Locked door requiring key |
| 2. Puzzle Room | Mental challenge | Key hidden, requires searching |
| 3. Setback | Complication | Trap or false path |
| 4. Climax | Big challenge | Main objective room |
| 5. Treasure | Reward | Victory condition |

Actual puzzle mechanics come in Phase 0.4 — this phase just defines the rooms and connections.

## Decisions Made

- Added `Description` property to `Adventure` domain type (not in original spec but consistent with the adventure.json manifest having a description field).
- JSON exits support both string format (`"north": "room-id"`) and object format (`"north": {"destination": "room-id", "barrier": "door-id"}`) via a custom `ExitDictionaryConverter`. Only destination is used in Phase 0.3; barrier support comes in 0.4.
- Used `Enum.TryParse<Direction>` for parsing direction strings from JSON — the enum values match JSON keys case-insensitively, avoiding a dependency on `Engine.DirectionParser`.
- Validation (exit references, item references, starting room, BFS reachability) runs on DTOs before constructing domain objects.
- DTOs live in `Framework.Content.Dtos` namespace, separate from domain entities.

## Out of Scope

- Puzzle/barrier logic (Phase 0.4)
- Dynamic content updates
- Content creation tools
- Localisation
