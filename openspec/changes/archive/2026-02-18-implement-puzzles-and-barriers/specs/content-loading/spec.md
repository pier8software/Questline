## MODIFIED Requirements

### Requirement: Loader constructs World from content files

The adventure loader SHALL read the JSON files and construct a World with rooms, exits, items, features, and barriers.

#### Scenario: Load adventure with barriers

- **WHEN** the adventure JSON contains a `barriers` array with an entry for "iron-door"
- **THEN** the loaded GameState SHALL contain a Barrier with Id "iron-door" and its properties

#### Scenario: Load adventure with room features

- **WHEN** a room definition contains a `features` array with an entry for "strange-symbols"
- **THEN** the loaded Room SHALL contain a Feature with that Id, name, keywords, and description

### Requirement: Five-room dungeon adventure

A complete 5-room dungeon adventure SHALL be defined in content files, following the classic 5-room dungeon structure (entrance, puzzle, setback, climax, reward). The puzzle room SHALL define barriers and features.

#### Scenario: Puzzle room has barrier and features

- **WHEN** the five-room-dungeon adventure is loaded
- **THEN** the puzzle room SHALL have an exit with a barrier reference and at least one examinable feature
