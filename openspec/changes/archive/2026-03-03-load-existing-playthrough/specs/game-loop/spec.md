## MODIFIED Requirements

### Requirement: Game displays initial room on start

The game SHALL prompt for a character name, display a welcome message including the character's name, and then display the starting room's name, description, available items, locked barrier descriptions, and exits when the game begins. Adventure content SHALL already be present in the database, deployed via `deploy-content` mode before the game is started. When loading an existing playthrough, the game SHALL skip character creation and display the current room instead of the starting room.

#### Scenario: Game startup with character creation

- **WHEN** the game starts and the player selects "New Game" and enters character name "Thorin"
- **THEN** the output SHALL contain a welcome message including "Thorin"
- **AND** the output SHALL contain the starting room name, description, and available exits

#### Scenario: Game startup displays items in starting room

- **WHEN** the game starts and the player selects "New Game" and the starting room contains items
- **THEN** the output SHALL display a "You can see:" line listing the item names

#### Scenario: Game startup displays locked barriers

- **WHEN** the game starts and the player selects "New Game" and the starting room has exits blocked by locked barriers
- **THEN** the output SHALL display each locked barrier's description

#### Scenario: Game startup with loaded playthrough

- **WHEN** the game starts and the player selects "Load Game" and loads a saved playthrough
- **THEN** the output SHALL display the current room name, description, exits, items, and locked barriers
- **AND** the game SHALL NOT prompt for character creation
