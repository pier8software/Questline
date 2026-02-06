# Questline Architecture

## Overview

Questline is designed with the eventual MUD platform in mind, even while delivering simpler single-player milestones. The architecture enforces separation between game logic and I/O, enabling the same command execution to work whether input comes from a local terminal or a network socket.

## Project Structure

```
Questline/
├── src/
│   ├── Questline/
│   │   ├── Cli/              # Entry point, game loop, terminal I/O
│   │   ├── Domain/           # Entities, value objects, game rules
│   │   ├── Engine/           # Parser, command pipeline, handlers
│   │   └── Framework/        # Persistence, serialisation, messaging
│   └── Questline.Infrastructure/   # IaC (future phases)
├── tests/
│   └── Questline.Tests/
└── content/
    └── adventures/
        └── five-room-dungeon/
```

### Namespace Responsibilities

**Questline.Cli**
- Application entry point
- Game loop orchestration
- Terminal input/output
- Dependency injection composition root

**Questline.Domain**
- Entity definitions (Room, Item, Player, Character, etc.)
- Value objects (Position, DamageRange, etc.)
- Game rules and invariants
- No dependencies on other namespaces

**Questline.Engine**
- Command parser (tokenisation, verb matching)
- Command dispatcher
- Command handlers (Go, Look, Get, Drop, Use, etc.)
- Game event generation

**Questline.Framework**
- JSON serialisation helpers
- Save/load functionality
- Content loading from adventure files
- Messaging/event pipeline (future phases)

## Component Diagram

```
┌─────────────────────────────────────────────────────────┐
│                      Game Shell                         │
│                       (Cli/)                            │
│              Terminal I/O, game loop                    │
└─────────────────┬───────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────────┐
│                   Command Pipeline                      │
│                      (Engine/)                          │
│         ┌─────────┐    ┌─────────────┐                  │
│         │ Parser  │───▶│  Dispatcher │                  │
│         └─────────┘    └──────┬──────┘                  │
│                               │                         │
│                   ┌───────────▼────────────┐            │
│                   │   Command Handlers     │            │
│                   │ (Go, Get, Look, etc.)  │            │
│                   └───────────┬────────────┘            │
└───────────────────────────────┼─────────────────────────┘
                                │
┌───────────────────────────────▼─────────────────────────┐
│                     Game State                          │
│                      (Domain/)                          │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────┐  │
│  │   World     │  │   Player    │  │  Quest/Session  │  │
│  │  (rooms,    │  │ (character, │  │    (current     │  │
│  │  entities)  │  │  inventory) │  │     state)      │  │
│  └─────────────┘  └─────────────┘  └─────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

## Key Design Decisions

### Command → Event Model

The parser produces a command object. Execution produces events/results that can be broadcast. This avoids tight coupling between parsing and response rendering, and enables multiplayer where command results must be sent to multiple clients.

```
Input: "get lamp"
  ↓
Parser: GetCommand { Target = "lamp" }
  ↓
Handler: Validates, modifies state
  ↓
Event: ItemPickedUp { Player = "...", Item = "lamp" }
  ↓
Renderer: "You pick up the brass lamp."
```

### World Model

Rooms are modelled as a graph with directional connections. Each room tracks:
- Description and metadata
- Items present
- Entities (players, NPCs) present
- Exits (direction → destination room)
- Barriers (locked doors, puzzles)

Locations are tagged as `Hub` or `Instanced` to support the future MUD model, even if Phase 0 only uses instanced content.

### Player State Persistence

Player and character state must be serialisable from day one. This includes:
- Character identity (name, race, class)
- Current location
- Inventory contents
- Quest/session progress

Save files are JSON, enabling easy inspection and debugging.

### Content as Data

Adventures are defined in JSON files, not hardcoded. This includes:
- Room definitions and connections
- Item definitions and properties
- Puzzle/barrier definitions
- Starting conditions

Behaviour that cannot be expressed declaratively (complex puzzle logic, special events) can be implemented as named handlers referenced from the content files.

### Inventory Abstraction

Inventory is modelled as a generic container. Player inventory and party inventory (future phases) use the same structure with different ownership semantics.

### Gear as Data

Weapons and equipment are data-driven:
- Damage ranges (e.g., 1-4, 1-10)
- Damage types (physical, fire, etc.)
- Special properties (effective against undead, etc.)

No hardcoded classes per item type.

### Extensible Parser

The parser uses a registration model for verbs rather than a switch statement. New commands can be added by registering a handler, without modifying existing code.

```csharp
// Example registration pattern
services.AddCommandHandler<GoCommandHandler>("go", "walk", "move");
services.AddCommandHandler<LookCommandHandler>("look", "examine", "inspect");
```

## Future Architecture Considerations

These are not implemented in Phase 0 but influence current design:

### Multiplayer (Phase 2+)

- Server-authoritative command execution
- WebSocket or TCP connections
- Command results broadcast to all party members
- Shared quest instance state

### MUD Platform (Phase 3)

- Persistent hub world separate from instanced quests
- Player presence tracking per room
- Grace period on disconnect before character despawns
- Party system with shared inventory
