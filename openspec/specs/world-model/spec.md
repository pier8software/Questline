# world-model Specification

## Purpose

Define the spatial model of the game world. Rooms are entities connected by directional exits forming a navigable graph. Players move through rooms using the `go` command and observe rooms using the `look` command.

## Requirements

### Requirement: Room entity with identity and description

A Room SHALL have a unique `Id`, a `Name`, a `Description`, an `Items` list (`ImmutableList<Item>`), and a `Features` list.

#### Scenario: Room exposes identity and descriptive properties

- **WHEN** a Room is created with Id "entrance", Name "Cave Entrance", and Description "A dark cave."
- **THEN** the Room's `Id` SHALL be "entrance", `Name` SHALL be "Cave Entrance", and `Description` SHALL be "A dark cave."

#### Scenario: Room with features

- **WHEN** a Room has a Feature with name "strange symbols" and keywords ["symbols", "carvings"]
- **THEN** the Room's Features list SHALL contain that feature

### Requirement: Directional exits link rooms

A Room SHALL have exits mapping a Direction (North, South, East, West, Up, Down) to an Exit value object. Each Exit SHALL have a Destination (room ID) and an optional BarrierId.

#### Scenario: Room with exits

- **WHEN** a Room has an exit North with destination "hallway"
- **THEN** the Room's exits SHALL contain a North entry whose Destination is "hallway" and whose BarrierId is null

#### Scenario: Room with barrier exit

- **WHEN** a Room has an exit North with destination "hallway" and BarrierId "iron-door"
- **THEN** the Room's exits SHALL contain a North entry whose Destination is "hallway" and whose BarrierId is "iron-door"

### Requirement: World is a room graph

The World (GameState) SHALL hold a collection of Rooms retrievable by ID, and a collection of Barriers retrievable by ID.

#### Scenario: Retrieve room by ID

- **WHEN** the World contains a Room with Id "entrance"
- **THEN** calling `GetRoom("entrance")` SHALL return that Room

#### Scenario: Retrieve nonexistent room

- **WHEN** the World does not contain a Room with Id "missing"
- **THEN** calling `GetRoom("missing")` SHALL throw an exception

#### Scenario: Retrieve barrier by ID

- **WHEN** the GameState contains a Barrier with Id "iron-door"
- **THEN** calling `GetBarrier("iron-door")` SHALL return that Barrier

#### Scenario: Retrieve barrier with null ID

- **WHEN** `GetBarrier(null)` is called
- **THEN** the result SHALL be null

### Requirement: WorldBuilder constructs world programmatically

WorldBuilder SHALL provide a fluent API to define rooms with exits, items, features, and barriers. The builder SHALL support `WithFeature(Feature)` on rooms and `WithBarrier(Barrier)` on the game. A `BuildState(playerId, startLocation)` method SHALL produce a complete GameState.

#### Scenario: Build a two-room world

- **WHEN** WorldBuilder defines rooms "start" and "end" with a North exit from "start" to "end"
- **THEN** the built World SHALL contain both rooms with the correct exit

#### Scenario: Build a room with barrier exit

- **WHEN** WorldBuilder defines a room with a North exit using `Exit("end", "iron-door")`
- **THEN** the built Room's North exit SHALL have Destination "end" and BarrierId "iron-door"

#### Scenario: Build a room with feature

- **WHEN** WorldBuilder defines a room with a Feature
- **THEN** the built Room's Features list SHALL contain that feature

#### Scenario: Build a game with barrier

- **WHEN** WorldBuilder defines a game with a Barrier and builds a GameState
- **THEN** the GameState SHALL contain the barrier retrievable by ID

### Requirement: Look command displays room information

The `look` command SHALL display the current room's name, description, and available exits.

#### Scenario: Look in a room with exits

- **WHEN** the player is in a room named "Cave Entrance" with exits North and East
- **THEN** the look result SHALL contain the room name, description, and both exit directions

### Requirement: Go command moves player

The `go <direction>` command SHALL move the player to the destination room when an exit exists in that direction.

#### Scenario: Move through existing exit

- **WHEN** the player is in "start" and executes `go north` and a North exit leads to "hallway"
- **THEN** the player's location SHALL be "hallway"

#### Scenario: Move through nonexistent exit

- **WHEN** the player is in "start" and executes `go north` but no North exit exists
- **THEN** an error result SHALL be returned and the player's location SHALL remain "start"
