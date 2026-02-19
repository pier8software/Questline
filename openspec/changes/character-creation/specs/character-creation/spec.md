## ADDED Requirements

### Requirement: New game prompts for character name

On starting a new game, the game SHALL prompt the player to enter a character name.

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

A new character SHALL have 6 ability scores (STR, INT, WIS, DEX, CON, CHA) and health values. Each ability score is rolled by summing 3d6, assigned in order. MaxHealth is 8 (Fighter hit die ceiling at level 1). CurrentHealth is a 1d8 roll.

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

Player (the human) and Character (the in-game avatar) SHALL be separate models. Player has an Id, a Character, a Location, and an Inventory.

#### Scenario: Player owns character

- **WHEN** a Player is created with a Character named "Thorin"
- **THEN** `Player.Character.Name` SHALL be "Thorin"

### Requirement: Welcome message uses character name

On starting a new game, the welcome message SHALL include the character's name.

#### Scenario: Personalised welcome

- **WHEN** a character named "Thorin" starts the adventure
- **THEN** the output SHALL contain "Thorin" in the welcome message
