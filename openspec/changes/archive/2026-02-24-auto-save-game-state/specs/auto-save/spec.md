## ADDED Requirements

### Requirement: Game state is automatically persisted after every request

The game state SHALL be automatically saved to the repository after every request handler completes, regardless of whether the request was a command or a query. Handlers SHALL NOT reference or depend on the repository directly.

#### Scenario: State is saved after a state-changing command

- **WHEN** a player executes a command that changes game state (e.g., moving to a new room)
- **THEN** the repository `Save` method SHALL be called with the updated game state after the handler returns

#### Scenario: State is saved after a read-only query

- **WHEN** a player executes a query that does not change game state (e.g., looking at the room)
- **THEN** the repository `Save` method SHALL still be called with the current game state after the handler returns

#### Scenario: Save occurs transparently via decorator

- **WHEN** any request handler processes a request
- **THEN** the `AutoSaveDecorator` SHALL invoke the inner handler first, then call `Save` on the repository
- **AND** the handler SHALL NOT have any dependency on or reference to the repository

### Requirement: Repository persists game state using upsert semantics

The `IGameStateRepository.Save(GameState)` method SHALL create a new document when no document exists for the current game session, and update the existing document when one already exists.

#### Scenario: First save creates a new document

- **WHEN** `Save` is called and no document exists for the current game session
- **THEN** a new document SHALL be inserted into the database containing the full game state

#### Scenario: Subsequent saves update the existing document

- **WHEN** `Save` is called and a document already exists for the current game session
- **THEN** the existing document SHALL be updated with the current game state

### Requirement: Persisted state includes all game data

The saved game state document SHALL contain the complete game state: player identity, character data, all rooms with their current items, all barriers with their unlock status, and adventure metadata.

#### Scenario: Saved state contains player and character data

- **WHEN** game state is saved for a character named "Thorin" (Human Fighter, Level 1) located in "entrance"
- **THEN** the persisted document SHALL contain the player ID, character name, race, class, level, experience, ability scores, hit points, location, and inventory

#### Scenario: Saved state contains room data with current items

- **WHEN** game state is saved and the "entrance" room contains a "rusty key"
- **THEN** the persisted document SHALL contain the room's ID, name, description, exits, and current item list including "rusty key"

#### Scenario: Saved state contains barrier unlock status

- **WHEN** game state is saved after the "iron door" barrier has been unlocked
- **THEN** the persisted document SHALL record the "iron door" barrier with `IsUnlocked` as true

#### Scenario: Saved state contains adventure metadata

- **WHEN** game state is saved for adventure "the-goblins-lair"
- **THEN** the persisted document SHALL contain the adventure ID and application version

### Requirement: World state is saved when a new game starts

When a new game begins, the adventure content SHALL be loaded from JSON and the full world state SHALL be saved to the repository as the initial game profile before character creation begins.

#### Scenario: Initial world snapshot saved on new game

- **WHEN** a player starts a new game with adventure "the-goblins-lair"
- **THEN** the world state (all rooms, barriers, and items) SHALL be saved to the repository before character creation begins

#### Scenario: Character saved to profile after creation

- **WHEN** character creation completes and produces a character
- **THEN** the character SHALL be saved to the existing game profile via the repository
