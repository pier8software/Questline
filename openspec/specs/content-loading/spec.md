# content-loading Specification

## Purpose

Define how adventure content (rooms, items, connections) is loaded from JSON data files at runtime. Content authoring is decoupled from code — adventures are defined as data, not hardcoded.

## Requirements

### Requirement: Adventures defined in JSON files

Adventure content SHALL live in `content/adventures/<adventure-name>/` with separate files for the manifest, rooms, and items.

#### Scenario: Adventure file structure

- **WHEN** an adventure "five-room-dungeon" exists
- **THEN** the directory SHALL contain `adventure.json` (manifest), `rooms.json`, and `items.json`

### Requirement: Adventure manifest defines metadata

The adventure manifest SHALL contain `id`, `name`, `description`, `startingRoomId`, and `version`.

#### Scenario: Load manifest

- **WHEN** `adventure.json` contains id "five-room-dungeon" and startingRoomId "entrance"
- **THEN** the loaded adventure SHALL have those values

### Requirement: Loader constructs World from content files

The adventure loader SHALL read the JSON files and construct a World with rooms, exits, and items.

#### Scenario: Load valid adventure

- **WHEN** the loader processes valid adventure files
- **THEN** the resulting World SHALL contain all defined rooms with their exits and items

### Requirement: Exit formats — string and object

Room exits SHALL support both a simple string format (`"north": "room-id"`) and an object format (`"north": {"destination": "room-id", "barrier": "barrier-id"}`).

#### Scenario: Simple string exit

- **WHEN** a room's exit is defined as `"north": "hallway"`
- **THEN** the exit SHALL point to destination "hallway" with no barrier

#### Scenario: Object exit with barrier

- **WHEN** a room's exit is defined as `"north": {"destination": "hallway", "barrier": "iron-door"}`
- **THEN** the exit SHALL point to destination "hallway" with barrier "iron-door"

### Requirement: Validation on load

The loader SHALL validate content on load: all exit destination room IDs must exist, all room item references must exist in items, the starting room must exist, and all rooms must be reachable from the starting room.

#### Scenario: Exit references missing room

- **WHEN** a room exit references a room ID that does not exist
- **THEN** loading SHALL fail with a clear error message

#### Scenario: Item reference missing

- **WHEN** a room references an item ID that does not exist in items.json
- **THEN** loading SHALL fail with a clear error message

#### Scenario: Starting room missing

- **WHEN** the manifest's startingRoomId does not match any room
- **THEN** loading SHALL fail with a clear error message

#### Scenario: Unreachable room

- **WHEN** a room is not reachable from the starting room via BFS traversal
- **THEN** loading SHALL fail with a clear error message

### Requirement: Invalid JSON produces clear errors

Malformed JSON or missing required fields SHALL produce clear error messages indicating the file and problem.

#### Scenario: Malformed JSON

- **WHEN** a content file contains invalid JSON
- **THEN** loading SHALL fail with an error message identifying the file

#### Scenario: Missing required field

- **WHEN** a room definition is missing a required field
- **THEN** loading SHALL fail with a helpful error message

### Requirement: Five-room dungeon adventure

A complete 5-room dungeon adventure SHALL be defined in content files, following the classic 5-room dungeon structure (entrance, puzzle, setback, climax, reward).

#### Scenario: Dungeon completeness

- **WHEN** the five-room-dungeon adventure is loaded
- **THEN** the World SHALL contain at least 5 rooms connected in a navigable graph
