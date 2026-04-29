## MODIFIED Requirements

### Requirement: Game displays initial room on start

The game SHALL prompt for a character name, display a welcome message including the character's name, and then display the starting room's name, description, available items, locked barrier descriptions, and exits when the game begins. Adventure content SHALL be seeded into the database before the game loop begins, as an explicit step of the game mode rather than a side-effect of application startup.

#### Scenario: Game startup with character creation

- **WHEN** the game starts and the player enters character name "Thorin"
- **THEN** the output SHALL contain a welcome message including "Thorin"
- **AND** the output SHALL contain the starting room name, description, and available exits

#### Scenario: Game startup displays items in starting room

- **WHEN** the game starts and the starting room contains items
- **THEN** the output SHALL display a "You can see:" line listing the item names

#### Scenario: Game startup displays locked barriers

- **WHEN** the game starts and the starting room has exits blocked by locked barriers
- **THEN** the output SHALL display each locked barrier's description

#### Scenario: Game mode seeds content before starting

- **WHEN** the application runs in game mode
- **THEN** adventure content SHALL be seeded into the database before the game loop begins
