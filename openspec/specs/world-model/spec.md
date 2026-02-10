# world-model Specification

## Purpose

Define the spatial model of the game world. Rooms are entities connected by directional exits forming a navigable graph. Players move through rooms using the `go` command and observe rooms using the `look` command.

## Requirements

### Requirement: Room entity with identity and description

A Room SHALL have a unique `Id`, a `Name`, and a `Description`.

#### Scenario: Room exposes identity and descriptive properties

- **WHEN** a Room is created with Id "entrance", Name "Cave Entrance", and Description "A dark cave."
- **THEN** the Room's `Id` SHALL be "entrance", `Name` SHALL be "Cave Entrance", and `Description` SHALL be "A dark cave."

### Requirement: Directional exits link rooms

A Room SHALL have exits mapping a Direction (North, South, East, West, Up, Down) to a destination room ID.

#### Scenario: Room with exits

- **WHEN** a Room has an exit North pointing to "hallway"
- **THEN** the Room's exits SHALL contain a North entry with destination "hallway"

### Requirement: World is a room graph

The World SHALL hold a collection of Rooms retrievable by ID.

#### Scenario: Retrieve room by ID

- **WHEN** the World contains a Room with Id "entrance"
- **THEN** calling `GetRoom("entrance")` SHALL return that Room

#### Scenario: Retrieve nonexistent room

- **WHEN** the World does not contain a Room with Id "missing"
- **THEN** calling `GetRoom("missing")` SHALL throw an exception

### Requirement: WorldBuilder constructs world programmatically

WorldBuilder SHALL provide a fluent API to define rooms with exits and build a World.

#### Scenario: Build a two-room world

- **WHEN** WorldBuilder defines rooms "start" and "end" with a North exit from "start" to "end"
- **THEN** the built World SHALL contain both rooms with the correct exit

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
