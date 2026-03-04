## ADDED Requirements

### Requirement: Playthroughs are associated with a player username

Each playthrough SHALL store the username of the player who created it. This username SHALL be set when the playthrough is created during character creation.

#### Scenario: New playthrough stores username

- **WHEN** a player with username "alice" completes character creation
- **THEN** the saved playthrough SHALL have the username "alice" associated with it

### Requirement: Playthroughs can be queried by username

The system SHALL support querying all playthroughs belonging to a specific player by username. The query SHALL return playthrough summaries including the playthrough ID, character name, adventure ID, and current location.

#### Scenario: Query returns matching playthroughs

- **WHEN** the system queries playthroughs for username "alice"
- **AND** "alice" has two saved playthroughs
- **THEN** the query SHALL return two playthrough summaries

#### Scenario: Query returns empty list for unknown username

- **WHEN** the system queries playthroughs for username "unknown"
- **AND** no playthroughs exist for "unknown"
- **THEN** the query SHALL return an empty list

## MODIFIED Requirements

### Requirement: Load restores game state

Loading a save SHALL restore the character, inventory, player position, and world state exactly as saved. Loading SHALL be initiated by selecting a saved playthrough from the start menu's "Load Game" option, and the game SHALL resume from the player's last location.

#### Scenario: Load restores character

- **WHEN** a save with character "Thorin" is loaded
- **THEN** the character's name, race, class, and stats SHALL match the saved values

#### Scenario: Load restores inventory

- **WHEN** a save with "rusty key" in inventory is loaded
- **THEN** the player's inventory SHALL contain "rusty key"

#### Scenario: Load restores position

- **WHEN** a save with current room "puzzle-room" is loaded
- **THEN** the player's location SHALL be "puzzle-room"

#### Scenario: Load restores world state

- **WHEN** a save with an unlocked barrier is loaded
- **THEN** the barrier SHALL remain unlocked

#### Scenario: Load resumes gameplay

- **WHEN** a saved playthrough is loaded from the start menu
- **THEN** the game SHALL transition to the playing phase
- **AND** the game SHALL display the current room details
