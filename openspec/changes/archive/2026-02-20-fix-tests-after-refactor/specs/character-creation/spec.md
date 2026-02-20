## MODIFIED Requirements

### Requirement: Character has base stats

A new character SHALL have 6 ability scores (STR, INT, WIS, DEX, CON, CHA) and hit points. Each ability score is rolled by summing 3d6, assigned in order: STR, INT, WIS, DEX, CON, CHA. MaxHitPoints is 8 (Fighter hit die ceiling at level 1). CurrentHitPoints is a 1d8 roll (character may start below max). Stats do NOT vary by race or class (out of scope for this feature). Ability scores and hit points SHALL be separate value objects.

#### Scenario: Rolled stats

- **WHEN** a default Human Fighter is created
- **THEN** STR, INT, WIS, DEX, CON, CHA SHALL each be an AbilityScore containing the sum of 3d6 (range 3-18)
- **AND** HitPoints.MaxHitPoints SHALL be 8
- **AND** HitPoints.CurrentHitPoints SHALL be a 1d8 roll (range 1-8)

### Requirement: Player and Character are separate models

Player (the human) and Character (the in-game avatar) SHALL be separate models. Player is a record with an Id and a Character. Character is a record created via a factory method with a Location and an Inventory. Location is mutated via a SetLocation method. Location and Inventory are properties of the Character, not the Player.

#### Scenario: Player owns character

- **WHEN** a Player is created with a Character named "Thorin"
- **THEN** `Player.Character.Name` SHALL be "Thorin"

#### Scenario: Character holds location

- **WHEN** a Character moves to room "dungeon-entrance"
- **THEN** `Player.Character.Location` SHALL be "dungeon-entrance" (set via `SetLocation`)

#### Scenario: Character holds inventory

- **WHEN** a Character picks up an item
- **THEN** the item SHALL be in `Player.Character.Inventory`
- **AND** `Player` SHALL NOT have a direct Inventory property

### Requirement: Stats command displays character information

The `stats` command SHALL display the character's name, race, class, level, and stats including ability scores and hit points.

#### Scenario: Stats output

- **WHEN** the player executes `stats`
- **THEN** the result SHALL contain the character name, race, class, level, ability scores, and hit point values
