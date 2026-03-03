# start-menu Specification

## Purpose

Define the start menu presented after login, allowing players to begin a new game or load an existing playthrough.

## Requirements

### Requirement: Start menu displays after login

After the player logs in, the game SHALL display a start menu with two options: "1. New Game" and "2. Load Game". The game SHALL NOT proceed to adventure selection or playthrough loading until the player makes a selection.

#### Scenario: Start menu is displayed after login

- **WHEN** the player successfully logs in
- **THEN** the game SHALL display a start menu with options "1. New Game" and "2. Load Game"

#### Scenario: Start menu waits for valid input

- **WHEN** the start menu is displayed and the player enters an invalid selection (e.g. "3" or "abc")
- **THEN** the game SHALL display an error message and continue to show the start menu prompt

### Requirement: New Game option starts adventure selection

When the player selects "1" (New Game) from the start menu, the game SHALL proceed to the adventure selection flow, followed by character creation, as per the existing flow.

#### Scenario: Player selects New Game

- **WHEN** the player enters "1" at the start menu
- **THEN** the game SHALL transition to the adventure selection phase
- **AND** the game SHALL display the list of available adventures

### Requirement: Load Game option displays saved playthroughs

When the player selects "2" (Load Game) from the start menu, the game SHALL query for all playthroughs belonging to the logged-in player and display them as a numbered list. Each entry SHALL show the character name, adventure name, and current location.

#### Scenario: Player selects Load Game with saved playthroughs

- **WHEN** the player enters "2" at the start menu
- **AND** the player has saved playthroughs
- **THEN** the game SHALL display a numbered list of saved playthroughs
- **AND** each entry SHALL include the character name, adventure name, and current location

#### Scenario: Player selects Load Game with no saved playthroughs

- **WHEN** the player enters "2" at the start menu
- **AND** the player has no saved playthroughs
- **THEN** the game SHALL display a message indicating no saved games were found
- **AND** the game SHALL return to the start menu

### Requirement: Selecting a saved playthrough resumes the adventure

When the player selects a saved playthrough from the list, the game SHALL load the full playthrough state and resume gameplay from the player's last location. The game SHALL display the current room details (name, description, exits, items, locked barriers) as if the player had just entered the room.

#### Scenario: Player selects a valid saved playthrough

- **WHEN** the player selects a saved playthrough by entering its number
- **THEN** the game SHALL load the playthrough state including character, inventory, location, and world state
- **AND** the game SHALL display the current room name, description, exits, items, and locked barriers
- **AND** the game SHALL transition to the playing phase

#### Scenario: Player enters invalid playthrough selection

- **WHEN** the player enters an invalid selection (e.g. a number not in the list)
- **THEN** the game SHALL display an error message and continue to show the playthrough selection prompt

### Requirement: Loaded playthrough retains all saved state

When a playthrough is loaded, the character stats, inventory, unlocked barriers, and room item state SHALL match the state at the time of the last save.

#### Scenario: Loaded playthrough has correct character stats

- **WHEN** a playthrough is loaded for a character named "Thorin" who is a Level 1 Human Fighter
- **THEN** the character's name SHALL be "Thorin", race SHALL be Human, class SHALL be Fighter, and level SHALL be 1

#### Scenario: Loaded playthrough has correct inventory

- **WHEN** a playthrough is loaded where the player had "rusty key" in inventory
- **THEN** the player's inventory SHALL contain "rusty key"

#### Scenario: Loaded playthrough has correct world state

- **WHEN** a playthrough is loaded where the "iron-door" barrier was unlocked
- **THEN** the barrier "iron-door" SHALL be unlocked
