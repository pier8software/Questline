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

#### Scenario: Invalid name

- **WHEN** the player enters an empty string, a name shorter than 2 characters, longer than 24 characters, or containing invalid characters
- **THEN** a validation error SHALL be returned with message: "Please give your character a name"

### Requirement: Default race and class

A new character SHALL be created with race Human and class Fighter by default.

#### Scenario: Default character template

- **WHEN** a character is created with name "Thorin"
- **THEN** the character's Race SHALL be Human and Class SHALL be Fighter

### Requirement: Character has base stats

A new character SHALL have 6 ability scores (STR, INT, WIS, DEX, CON, CHA) and health values. Each ability score is rolled by summing 3d6, assigned in order: STR, INT, WIS, DEX, CON, CHA. MaxHealth is 8 (Fighter hit die ceiling at level 1). CurrentHealth is a 1d8 roll (character may start below max). Stats do NOT vary by race or class (out of scope for this feature).

#### Scenario: Rolled stats

- **WHEN** a default Human Fighter is created
- **THEN** STR, INT, WIS, DEX, CON, CHA SHALL each be the sum of 3d6 (range 3-18)
- **AND** MaxHealth SHALL be 8
- **AND** CurrentHealth SHALL be a 1d8 roll (range 1-8)

### Requirement: Stats command displays character information

The `stats` command SHALL display the character's name, race, class, level, and stats.

#### Scenario: Stats output

- **WHEN** the player executes `stats`
- **THEN** the result SHALL contain the character name, race, class, level, and all stat values

### Requirement: Player and Character are separate models

Player (the human) and Character (the in-game avatar) SHALL be separate models. Player has an Id, a Character, and a Location.

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
public record Character(
    string Name,
    Race Race,
    CharacterClass Class,
    int Level = 1,
    int Experience = 0,
    CharacterStats? Stats = null);

public enum Race { Human, Elf, Dwarf, Halfling }
public enum CharacterClass { Fighter, Wizard, Rogue, Cleric }

public record CharacterStats(
    int MaxHealth,
    int CurrentHealth,
    int STR,
    int INT,
    int WIS,
    int DEX,
    int CON,
    int CHA);
```

### Player vs Character Separation

```csharp
public class Player
{
    public required Guid Id { get; init; }
    public required Character Character { get; init; }
    public required string Location { get; set; }
}
```

### Dice Rolling

Stats use dice notation NdX: roll N dice with X sides and sum the results. Character creation uses 3d6 for each ability score and 1d8 for starting health.

### Game Start Flow

```
1. New Game / Continue / Quit
2. (New Game) Enter name -> validate -> create default Human Fighter
3. Roll ability scores (3d6 x 6) and health (1d8)
4. Welcome message with character name
5. Display starting room
```
