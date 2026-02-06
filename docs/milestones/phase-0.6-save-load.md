# Phase 0.6: Save and Load

## Status

Not Started

## Objective

Implement save and load functionality. Player can save progress at any point and resume later. This forces proper serialisation of game state, which is foundational for multiplayer persistence.

## Acceptance Criteria

### Save
- [ ] `save` command persists current game state
- [ ] Save file written to user-accessible location
- [ ] Save includes character, inventory, location, world state (unlocked barriers)
- [ ] Confirmation message shown on successful save
- [ ] Overwriting existing save prompts for confirmation

### Load
- [ ] "Continue" at main menu loads saved game
- [ ] Character restored with correct stats
- [ ] Inventory restored with all items
- [ ] Player position restored to saved room
- [ ] World state restored (barriers remain unlocked)
- [ ] Missing or corrupted save produces clear error

### Auto-Save (Optional)
- [ ] Game auto-saves on quit (if not explicitly declined)

## Implementation Notes

### Save File Location

Use platform-appropriate location:

```csharp
public static class SavePaths
{
    public static string SaveDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Questline",
        "saves"
    );
    
    public static string SaveFile(string characterName) => 
        Path.Combine(SaveDirectory, $"{SanitizeFileName(characterName)}.json");
}
```

### Save State Model

```csharp
public class SaveState
{
    public required string Version { get; init; }
    public required DateTime SavedAt { get; init; }
    public required string AdventureId { get; init; }
    public required CharacterSave Character { get; init; }
    public required string CurrentRoomId { get; init; }
    public required List<string> InventoryItemIds { get; init; }
    public required WorldStateSave WorldState { get; init; }
}

public class CharacterSave
{
    public required string Name { get; init; }
    public required Race Race { get; init; }
    public required CharacterClass Class { get; init; }
    public required int Level { get; init; }
    public required int Experience { get; init; }
    public required CharacterStats Stats { get; init; }
}

public class WorldStateSave
{
    public required List<string> UnlockedBarrierIds { get; init; }
    public required Dictionary<string, List<string>> RoomItems { get; init; }
}
```

### Versioning

Include version in save file for forward compatibility:

```json
{
  "version": "0.6.0",
  "savedAt": "2024-01-15T14:30:00Z",
  "adventureId": "five-room-dungeon",
  "character": { ... },
  "currentRoomId": "puzzle-room",
  "inventoryItemIds": ["rusty-key"],
  "worldState": {
    "unlockedBarrierIds": ["iron-door"],
    "roomItems": {
      "entrance": [],
      "treasure-room": ["gold-chalice"]
    }
  }
}
```

Future versions can include migration logic if schema changes.

### Save Service

```csharp
public interface ISaveService
{
    Task SaveAsync(GameSession session);
    Task<GameSession?> LoadAsync(string characterName);
    Task<IReadOnlyList<SaveInfo>> ListSavesAsync();
    Task DeleteAsync(string characterName);
}

public class SaveInfo
{
    public required string CharacterName { get; init; }
    public required DateTime SavedAt { get; init; }
    public required string AdventureId { get; init; }
}
```

### Restoration Flow

1. Load save file JSON
2. Deserialise to `SaveState`
3. Load adventure content from `AdventureId`
4. Create `Character` from `CharacterSave`
5. Reconstruct `Inventory` from `InventoryItemIds` + adventure items
6. Apply `WorldState` (unlock barriers, position items)
7. Place player in `CurrentRoomId`
8. Resume game loop

### Error Handling

Possible failure modes:

| Error | Handling |
|-------|----------|
| File not found | "No saved game found. Starting new game?" |
| JSON parse error | "Save file corrupted. Cannot load." |
| Version mismatch | Attempt migration, or "Save from incompatible version." |
| Adventure not found | "Adventure '[id]' not installed." |
| Invalid room reference | "Save file references unknown location." |

### Test Approach

```csharp
[Fact]
public async Task Save_PersistsCharacterState()
{
    // Arrange: game session with character
    // Act: save
    // Assert: file exists with correct character data
}

[Fact]
public async Task Load_RestoresCharacterState()
{
    // Arrange: saved game file
    // Act: load
    // Assert: character matches saved state
}

[Fact]
public async Task Load_RestoresInventory()
{
    // Arrange: save with items in inventory
    // Act: load
    // Assert: inventory contains saved items
}

[Fact]
public async Task Load_RestoresWorldState()
{
    // Arrange: save with unlocked barrier
    // Act: load
    // Assert: barrier remains unlocked
}

[Fact]
public async Task Load_CorruptedFile_ReturnsError()
{
    // Arrange: malformed JSON file
    // Act: attempt load
    // Assert: clear error, no crash
}
```

### Integration Test

End-to-end scenario:

```csharp
[Fact]
public async Task SaveLoadRoundTrip_PreservesGameState()
{
    // Arrange: play through part of adventure
    //   - pick up key
    //   - unlock door
    //   - move to room 2
    
    // Act: save, create new session, load
    
    // Assert: 
    //   - player in room 2
    //   - key in inventory
    //   - door unlocked
}
```

## Out of Scope

- Multiple save slots per character
- Cloud saves
- Save file encryption
- Automatic backup rotation
