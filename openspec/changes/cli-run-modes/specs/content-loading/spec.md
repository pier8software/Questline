## MODIFIED Requirements

### Requirement: Loader constructs World from content files

The adventure loader SHALL read the JSON files and construct a World with rooms, exits, items, features, and barriers. Content seeding SHALL be an explicit operation that can be invoked independently by any run mode, not a side-effect of application startup.

#### Scenario: Load valid adventure

- **WHEN** the loader processes valid adventure files
- **THEN** the resulting World SHALL contain all defined rooms with their exits and items

#### Scenario: Load adventure with barriers

- **WHEN** the adventure JSON contains a `barriers` array with an entry for "iron-door"
- **THEN** the loaded GameState SHALL contain a Barrier with Id "iron-door" and its properties

#### Scenario: Load adventure with room features

- **WHEN** a room definition contains a `features` array with an entry for "strange-symbols"
- **THEN** the loaded Room SHALL contain a Feature with that Id, name, keywords, and description

#### Scenario: Content seeding invoked independently

- **WHEN** `ContentSeeder.SeedAdventure` is called outside of the game loop
- **THEN** it SHALL load and store adventure content in the database without requiring game engine services
