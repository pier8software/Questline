# Phase 0.5: Character Creation

## Status

Not Started

## Objective

Implement character creation flow. Player enters their character name, and the game applies a default race/class combination under the covers. This establishes the character model for future progression mechanics.

## Acceptance Criteria

### New Game Flow
- [ ] On launch, player chooses "New Game" or "Continue"
- [ ] New Game prompts for character name
- [ ] Name is validated (non-empty, reasonable length)
- [ ] Character created with default race (Human) and class (Fighter)
- [ ] Game begins in starting room with welcome message

### Continue Flow
- [ ] Continue loads existing character (save/load in 0.6)
- [ ] If no save exists, prompts to start new game

### Character Model
- [ ] Character has name, race, class
- [ ] Character has base stats derived from race/class
- [ ] `stats` command displays character information

### Welcome Experience
- [ ] First room displays atmospheric introduction
- [ ] Character name used in welcome: "Welcome, brave [Name]..."

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

public enum Race
{
    Human,
    Elf,
    Dwarf,
    Halfling
    // Extensible for future
}

public enum CharacterClass
{
    Fighter,
    Wizard,
    Rogue,
    Cleric
    // Extensible for future
}

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

### Default Character Template

Human Fighter baseline:

```csharp
public static Character CreateDefault(string name) => new()
{
    Name = name,
    Race = Race.Human,
    Class = CharacterClass.Fighter,
    Stats = new CharacterStats
    {
        MaxHealth = 20,
        CurrentHealth = 20,
        Strength = 14,
        Dexterity = 12,
        Intelligence = 10,
        Wisdom = 10
    }
};
```

### Name Validation

- Minimum 2 characters
- Maximum 24 characters
- Alphanumeric and spaces only
- No leading/trailing whitespace
- Reject obviously inappropriate names (basic profanity filter optional)

### Game Start Flow

```
╔════════════════════════════════════╗
║          QUESTLINE                 ║
║    A Text Adventure                ║
╠════════════════════════════════════╣
║  1. New Game                       ║
║  2. Continue                       ║
║  3. Quit                           ║
╚════════════════════════════════════╝

> 1

What is your name, adventurer?
> Thorin

Welcome, brave Thorin! Your adventure begins...

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Cave Entrance
Daylight filters into a narrow cave mouth. The smell of damp 
earth fills your nostrils. A rusty key lies on the ground.

Exits: north

>
```

### Stats Command

```
> stats

═══ THORIN ═══
Human Fighter
Level 1 (0 XP)

Health: 20/20
Strength: 14
Dexterity: 12
Intelligence: 10
Wisdom: 10
```

### Player vs Character

Clarify the model distinction:

- **Player**: The human at the keyboard. Owns account (future), preferences.
- **Character**: The in-game avatar. Has stats, inventory, location.

For Phase 0, these are somewhat conflated, but the model should separate them:

```csharp
public class Player
{
    public required Character Character { get; init; }
    public Inventory Inventory { get; init; } = new();
    public required string CurrentRoomId { get; set; }
}
```

### Test Approach

```csharp
[Fact]
public void CreateCharacter_ValidName_CreatesHumanFighter()
{
    // Arrange/Act
    var character = CharacterFactory.Create("Thorin");
    
    // Assert
    character.Name.ShouldBe("Thorin");
    character.Race.ShouldBe(Race.Human);
    character.Class.ShouldBe(CharacterClass.Fighter);
}

[Fact]
public void CreateCharacter_EmptyName_ReturnsValidationError()
{
    // Arrange/Act
    var result = CharacterFactory.TryCreate("");
    
    // Assert
    result.IsSuccess.ShouldBeFalse();
    result.Error.ShouldContain("name");
}

[Fact]
public void Stats_DisplaysCharacterInfo()
{
    // Arrange: player with character
    // Act: execute "stats"
    // Assert: output contains name, race, class, stats
}
```

## Out of Scope

- Race/class selection UI (future enhancement)
- Different stat arrays per race/class (Phase 1)
- Abilities/skills (Phase 1)
- Multiple characters per player (architecture supports, but not implemented)
