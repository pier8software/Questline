## MODIFIED Requirements

### Requirement: Room entity with identity and description

A Room SHALL have a unique `Id`, a `Name`, a `Description`, an `Items` inventory, and a `Features` list.

#### Scenario: Room with features

- **WHEN** a Room has a Feature with name "strange symbols" and keywords ["symbols", "carvings"]
- **THEN** the Room's Features list SHALL contain that feature

### Requirement: World is a room graph

The World (GameState) SHALL hold a collection of Rooms retrievable by ID, and a collection of Barriers retrievable by ID.

#### Scenario: Retrieve barrier by ID

- **WHEN** the GameState contains a Barrier with Id "iron-door"
- **THEN** calling `GetBarrier("iron-door")` SHALL return that Barrier

#### Scenario: Retrieve barrier with null ID

- **WHEN** `GetBarrier(null)` is called
- **THEN** the result SHALL be null

### Requirement: WorldBuilder constructs world programmatically

WorldBuilder SHALL provide a fluent API to define rooms with exits, items, features, and barriers. The builder SHALL support `WithFeature(Feature)` on rooms and `WithBarrier(Barrier)` on the game. A `BuildState(playerId, startLocation)` method SHALL produce a complete GameState.

#### Scenario: Build a room with feature

- **WHEN** WorldBuilder defines a room with a Feature
- **THEN** the built Room's Features list SHALL contain that feature

#### Scenario: Build a game with barrier

- **WHEN** WorldBuilder defines a game with a Barrier and builds a GameState
- **THEN** the GameState SHALL contain the barrier retrievable by ID
