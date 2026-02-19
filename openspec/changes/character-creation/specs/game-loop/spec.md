## MODIFIED Requirements

### Requirement: Game displays initial room on start

The game SHALL prompt for a character name, display a welcome message including the character's name, and then display the starting room's name, description, and exits when the game begins.

#### Scenario: Game startup with character creation

- **WHEN** the game starts and the player enters character name "Thorin"
- **THEN** the output SHALL contain a welcome message including "Thorin"
- **AND** the output SHALL contain the starting room name, description, and available exits
