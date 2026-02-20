## MODIFIED Requirements

### Requirement: Game displays initial room on start

The game SHALL prompt for a character name, display a welcome message including the character's name, and then display the starting room's name, description, available items, locked barrier descriptions, and exits when the game begins.

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

### Requirement: EOF terminates gracefully

The game SHALL exit gracefully when input reaches end-of-file (e.g. piped input or Ctrl+D), including during character setup.

#### Scenario: EOF on input during game loop

- **WHEN** the input stream reaches EOF during the game loop
- **THEN** the game SHALL terminate without error

#### Scenario: EOF on input during character setup

- **WHEN** the input stream reaches EOF during character setup (before a character is created)
- **THEN** the game SHALL terminate without error and without entering the game loop
