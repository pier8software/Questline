# save-and-load Specification

## Purpose

Define save and load functionality. Players can persist progress at any point and resume later. This forces proper serialisation of game state, which is foundational for future multiplayer persistence.

## Requirements

### Requirement: Save command persists game state

The `save` command SHALL persist the current game state to a JSON file in a platform-appropriate location.

#### Scenario: Save game

- **WHEN** the player executes `save`
- **THEN** a save file SHALL be written containing character, inventory, location, and world state

#### Scenario: Save confirmation

- **WHEN** the save completes successfully
- **THEN** a confirmation message SHALL be displayed

### Requirement: Save includes all relevant state

The save file SHALL include character data (name, race, class, level, stats), inventory item IDs, current room ID, world state (unlocked barriers, item positions), and adventure ID.

#### Scenario: Save contains character

- **WHEN** a game with character "Thorin" (Human Fighter, Level 1) is saved
- **THEN** the save file SHALL contain the character's name, race, class, level, and stats

#### Scenario: Save contains world state

- **WHEN** a game is saved after unlocking the iron door
- **THEN** the save file SHALL record the iron door as unlocked

### Requirement: Load restores game state

Loading a save SHALL restore the character, inventory, player position, and world state exactly as saved.

#### Scenario: Load restores character

- **WHEN** a save with character "Thorin" is loaded
- **THEN** the character's name, race, class, and stats SHALL match the saved values

#### Scenario: Load restores inventory

- **WHEN** a save with "rusty key" in inventory is loaded
- **THEN** the player's inventory SHALL contain "rusty key"

#### Scenario: Load restores position

- **WHEN** a save with current room "puzzle-room" is loaded
- **THEN** the player's location SHALL be "puzzle-room"

#### Scenario: Load restores world state

- **WHEN** a save with an unlocked barrier is loaded
- **THEN** the barrier SHALL remain unlocked

### Requirement: Save file versioning

The save file SHALL include a version string for forward compatibility.

#### Scenario: Version in save file

- **WHEN** a game is saved
- **THEN** the save file SHALL contain a "version" field

### Requirement: Error handling on load

Missing, corrupted, or incompatible save files SHALL produce clear error messages.

#### Scenario: Missing save file

- **WHEN** the player attempts to load a nonexistent save
- **THEN** a clear error message SHALL be displayed (e.g. "No saved game found")

#### Scenario: Corrupted save file

- **WHEN** the save file contains invalid JSON
- **THEN** a clear error message SHALL be displayed without crashing

#### Scenario: Unknown adventure

- **WHEN** the save references an adventure not installed
- **THEN** a clear error message SHALL be displayed

## Implementation Notes

### SaveState Model

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

public class WorldStateSave
{
    public required List<string> UnlockedBarrierIds { get; init; }
    public required Dictionary<string, List<string>> RoomItems { get; init; }
}
```

### Save File Location

Platform-appropriate: `Environment.SpecialFolder.LocalApplicationData/Questline/saves/`

### Restoration Flow

1. Load save file JSON
2. Deserialise to SaveState
3. Load adventure content from AdventureId
4. Reconstruct Character, Inventory, WorldState
5. Place player in saved room
6. Resume game loop
