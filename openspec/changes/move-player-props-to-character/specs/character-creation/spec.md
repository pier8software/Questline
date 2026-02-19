## MODIFIED Requirements

### Requirement: Player and Character are separate models

Player (the human) and Character (the in-game avatar) SHALL be separate models. Player has an Id and a Character. Character has a Location and an Inventory. Location and Inventory are properties of the Character, not the Player.

#### Scenario: Player owns character

- **WHEN** a Player is created with a Character named "Thorin"
- **THEN** `Player.Character.Name` SHALL be "Thorin"

#### Scenario: Character holds location

- **WHEN** a Character moves to room "dungeon-entrance"
- **THEN** `Player.Character.Location` SHALL be "dungeon-entrance"

#### Scenario: Character holds inventory

- **WHEN** a Character picks up an item
- **THEN** the item SHALL be in `Player.Character.Inventory`
- **AND** `Player` SHALL NOT have a direct Inventory property

## MODIFIED Implementation Notes

### Character Model

```csharp
public class Character
{
    public required string Location { get; set; }
    public Inventory Inventory { get; init; } = new();
}
```

### Player vs Character Separation

```csharp
public class Player
{
    public required string Id { get; init; }
    public required Character Character { get; init; }
}
```
