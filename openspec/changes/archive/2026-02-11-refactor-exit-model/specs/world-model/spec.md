## MODIFIED Requirements

### Requirement: Directional exits link rooms

A Room SHALL have exits mapping a Direction (North, South, East, West, Up, Down) to an Exit value object. Each Exit SHALL have a Destination (room ID) and an optional BarrierId.

#### Scenario: Room with exits

- **WHEN** a Room has an exit North with destination "hallway"
- **THEN** the Room's exits SHALL contain a North entry whose Destination is "hallway" and whose BarrierId is null

#### Scenario: Room with barrier exit

- **WHEN** a Room has an exit North with destination "hallway" and BarrierId "iron-door"
- **THEN** the Room's exits SHALL contain a North entry whose Destination is "hallway" and whose BarrierId is "iron-door"

### Requirement: WorldBuilder constructs world programmatically

WorldBuilder SHALL provide a fluent API to define rooms with exits and build a World. The `WithExit` method SHALL accept a direction and destination string for simple exits, or a direction and Exit object for exits with additional data.

#### Scenario: Build a two-room world

- **WHEN** WorldBuilder defines rooms "start" and "end" with a North exit from "start" to "end"
- **THEN** the built World SHALL contain both rooms with the correct exit

#### Scenario: Build a room with barrier exit

- **WHEN** WorldBuilder defines a room with a North exit using `Exit("end", "iron-door")`
- **THEN** the built Room's North exit SHALL have Destination "end" and BarrierId "iron-door"
