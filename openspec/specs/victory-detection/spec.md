# victory-detection Specification

## Purpose

Define how the game detects and celebrates victory. When a player reaches a room tagged as a victory room, the game displays a victory message with completion statistics and offers replay.

## Requirements

### Requirement: Room victory tag

Rooms SHALL support an `isVictory` tag in content JSON. When a player enters a victory-tagged room, a VictoryEvent SHALL be triggered.

#### Scenario: Enter victory room

- **WHEN** the player moves into a room with `isVictory: true`
- **THEN** a VictoryEvent SHALL be returned

#### Scenario: Non-victory room

- **WHEN** the player moves into a room without the victory tag
- **THEN** no VictoryEvent SHALL be returned

### Requirement: Victory message with summary

The victory display SHALL include a congratulations message, the character's name, and completion statistics (rooms explored, items collected).

#### Scenario: Victory display

- **WHEN** a VictoryEvent is triggered
- **THEN** the output SHALL contain "VICTORY", the character's name, rooms explored count, and items collected count

### Requirement: Completion time tracking (optional)

The game MAY track and display the time elapsed since game start.

#### Scenario: Time displayed

- **WHEN** the victory screen is displayed
- **THEN** the elapsed time MAY be shown

### Requirement: Replay option

After victory, the player SHALL be offered the option to play again or quit.

#### Scenario: Play again

- **WHEN** the player chooses to play again after victory
- **THEN** a new game SHALL start

#### Scenario: Quit after victory

- **WHEN** the player chooses to quit after victory
- **THEN** the game SHALL exit gracefully

## Implementation Notes

### Content: Victory Tag

```json
{
  "id": "treasure-room",
  "name": "The Treasure Chamber",
  "description": "Mountains of gold glitter in the torchlight...",
  "isVictory": true
}
```

### VictoryEvent

```csharp
if (room.IsVictory)
{
    return new VictoryEvent(CalculateStats());
}
```

### Victory Display Format

```
═══════════════════════════════════════════════════
                    VICTORY!
═══════════════════════════════════════════════════

Congratulations, Thorin! You have conquered the
Goblin's Lair and claimed the treasure!

Rooms explored: 5/5
Items collected: 3
Time: 12 minutes

═══════════════════════════════════════════════════

Play again? (y/n)
```
