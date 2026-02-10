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

#### Scenario: Empty name

- **WHEN** the player enters an empty string
- **THEN** a validation error SHALL be returned

#### Scenario: Name too long

- **WHEN** the player enters a name longer than 24 characters
- **THEN** a validation error SHALL be returned

### Requirement: Default race and class

A new character SHALL be created with race Human and class Fighter by default.

#### Scenario: Default character template

- **WHEN** a character is created with name "Thorin"
- **THEN** the character's Race SHALL be Human and Class SHALL be Fighter

### Requirement: Character has base stats

A new character SHALL have base stats (MaxHealth, CurrentHealth, Strength, Dexterity, Intelligence, Wisdom) derived from the default race/class combination.

#### Scenario: Default Human Fighter stats

- **WHEN** a default Human Fighter is created
- **THEN** MaxHealth SHALL be 20, Strength 14, Dexterity 12, Intelligence 10, Wisdom 10

### Requirement: Stats command displays character information

The `stats` command SHALL display the character's name, race, class, level, and stats.

#### Scenario: Stats output

- **WHEN** the player executes `stats`
- **THEN** the result SHALL contain the character name, "Human Fighter", "Level 1", and all stat values

### Requirement: Player and Character are separate models

Player (the human) and Character (the in-game avatar) SHALL be separate models. Player has a Character, an Inventory, and a current room.

#### Scenario: Player owns character

- **WHEN** a Player is created with a Character named "Thorin"
- **THEN** `Player.Character.Name` SHALL be "Thorin"

### Requirement: Welcome message uses character name

On starting a new game, the welcome message SHALL include the character's name.

#### Scenario: Personalised welcome

- **WHEN** a character named "Thorin" starts the adventure
- **THEN** the output SHALL contain "Thorin" in the welcome message

## Implementation Notes

### Character Model

```csharp
public class Character
{
    public required string Name { get; init; }
    public required Race Race { get; init; }
    public required CharacterClass Class { get; init; }
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public CharacterStats Stats { get; init; } = new();
}

public enum Race { Human, Elf, Dwarf, Halfling }
public enum CharacterClass { Fighter, Wizard, Rogue, Cleric }

public class CharacterStats
{
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
}
```

### Player vs Character Separation

```csharp
public class Player
{
    public required Character Character { get; init; }
    public Inventory Inventory { get; init; } = new();
    public required string CurrentRoomId { get; set; }
}
```

### Game Start Flow

```
1. New Game / Continue / Quit
2. (New Game) Enter name -> validate -> create default Human Fighter
3. Welcome message with character name
4. Display starting room
```
