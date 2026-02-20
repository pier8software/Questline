# character-creation Specification

## Purpose

Define the character creation flow. On starting a new game, the player enters a character name and receives a default Human Fighter. This establishes the character model for future progression mechanics and separates Player identity from Character identity.

## Requirements

### Requirement: New game prompts for character name

On selecting "New Game", the game SHALL prompt the player to enter a character name.

#### Scenario: Name entry

- **WHEN** the player starts a new game
- **THEN** the game SHALL prompt "What is your name, adventurer?" (or similar)

### Requirement: Name validation

The character name SHALL be validated: non-empty, 2-24 characters, alphanumeric and spaces only, no leading/trailing whitespace.

#### Scenario: Valid name

- **WHEN** the player enters "Thorin"
- **THEN** the character SHALL be created with name "Thorin"

#### Scenario: Invalid name - empty

- **WHEN** the player enters an empty string
- **THEN** a validation error SHALL be returned with message "Please give your character a name"

#### Scenario: Invalid name - too short

- **WHEN** the player enters "A"
- **THEN** a validation error SHALL be returned with message "Please give your character a name"

#### Scenario: Invalid name - too long

- **WHEN** the player enters a name longer than 24 characters
- **THEN** a validation error SHALL be returned with message "Please give your character a name"

#### Scenario: Invalid name - special characters

- **WHEN** the player enters "Th@rin!"
- **THEN** a validation error SHALL be returned with message "Please give your character a name"

#### Scenario: Invalid name - leading or trailing whitespace

- **WHEN** the player enters " Thorin " (with leading/trailing spaces)
- **THEN** a validation error SHALL be returned with message "Please give your character a name"

### Requirement: Default race and class

A new character SHALL be created with race Human and class Fighter by default.

#### Scenario: Default character template

- **WHEN** a character is created with name "Thorin"
- **THEN** the character's Race SHALL be Human and Class SHALL be Fighter

### Requirement: Character has base stats

A new character SHALL have 6 ability scores (STR, INT, WIS, DEX, CON, CHA) and hit points. Each ability score is rolled by summing 3d6, assigned in order: STR, INT, WIS, DEX, CON, CHA. MaxHitPoints is 8 (Fighter hit die ceiling at level 1). CurrentHitPoints is a 1d8 roll (character may start below max). Stats do NOT vary by race or class (out of scope for this feature). Ability scores and hit points SHALL be separate value objects.

#### Scenario: Rolled stats

- **WHEN** a default Human Fighter is created
- **THEN** STR, INT, WIS, DEX, CON, CHA SHALL each be an AbilityScore containing the sum of 3d6 (range 3-18)
- **AND** HitPoints.MaxHitPoints SHALL be 8
- **AND** HitPoints.CurrentHitPoints SHALL be a 1d8 roll (range 1-8)

### Requirement: Stats command displays character information

The `stats` command SHALL display the character's name, race, class, level, and stats including ability scores and hit points.

#### Scenario: Stats output

- **WHEN** the player executes `stats`
- **THEN** the result SHALL contain the character name, race, class, level, ability scores, and hit point values

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

### Requirement: Welcome message uses character name

On starting a new game, the welcome message SHALL include the character's name.

#### Scenario: Personalised welcome

- **WHEN** a character named "Thorin" starts the adventure
- **THEN** the output SHALL contain "Thorin" in the welcome message

## Implementation Notes

### Character Model

```csharp
public record Character
{
    public string Name { get; private init; }
    public Race Race { get; private init; }
    public CharacterClass Class { get; private init; }
    public int Level { get; private init; }
    public int Experience { get; init; }
    public AbilityScores AbilityScores { get; init; }
    public HitPoints HitPoints { get; private init; }
    public string Location { get; private set; }
    public Inventory Inventory { get; private init; } = new();

    public static Character Create(string name, Race race, CharacterClass characterClass,
        HitPoints hitPoints, AbilityScores abilityScores, string location = "");
    public void SetLocation(string locationId);
}

public record HitPoints(int MaxHitPoints, int CurrentHitPoints);
public record AbilityScores(
    AbilityScore Strength, AbilityScore Intelligence, AbilityScore Wisdom,
    AbilityScore Dexterity, AbilityScore Constitution, AbilityScore Charisma);
public record AbilityScore(int Score);

public enum Race { Human, Elf, Dwarf, Halfling }
public enum CharacterClass { Fighter, Wizard, Rogue, Cleric }
```

### Player vs Character Separation

```csharp
public record Player(string Id, Character Character);
```

### Name Validation

```csharp
// FluentValidation-based validator
public class CharacterNameValidator : AbstractValidator<CharacterName>
public record CharacterName(string Name);
```

### Character Creation State Machine

```csharp
public class CharacterCreationStateMachine(IDice dice)
{
    public IResponse ProcessInput(string? input);
}

// States: PendingAbilityScores → PendingClassSelection → PendingRaceSelection
//       → PendingHitPoints → PendingCharacterName → Complete
```

### Dice Rolling

Stats use dice notation NdX: roll N dice with X sides and sum the results. Character creation uses 3d6 for each ability score and 1d8 for starting health.

### Game Start Flow

```
1. Welcome message
2. Hit enter to start character creation
3. Roll ability scores (3d6 x 6) — automatic
4. Select class (Fighter)
5. Select race (Human)
6. Roll hit points (1d8) — automatic
7. Enter character name → validate
8. Character creation complete, launch game
```
