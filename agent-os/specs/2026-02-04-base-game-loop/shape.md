# Base Game Loop — Shaping Notes

## Problem

Need a foundational game loop that can support a text adventure game. This is the core infrastructure that everything else builds on.

## Appetite

Small batch — this is foundational plumbing, not the game itself.

## Solution

Classic REPL pattern used by Zork, Colossal Cave Adventure, and other text adventures:

1. **Read** — Get player input from console
2. **Parse** — Convert text to structured command
3. **Execute** — Update game state based on command
4. **Print** — Display result to player
5. **Loop** — Repeat until quit

## Design Decisions

### Immutable State
Using immutable records for game state. Commands return new state rather than mutating. This:
- Makes state changes explicit
- Enables undo/save/load features later
- Prevents accidental state corruption

### Simple Command Structure
Starting with verb + optional noun: `Command(Verb, Noun?)`
- Covers 90% of adventure game commands
- Easy to parse and understand
- Can extend later for "put X in Y" patterns

### Separate Parser and Handler
Parser converts text to commands. Handler executes commands.
- Single responsibility
- Parser can be tested without game logic
- Handler can be tested without string parsing

## Rabbit Holes

- **Don't** build a full natural language parser
- **Don't** implement save/load yet
- **Don't** add room/item definitions yet (hardcode minimal test data)
- **Don't** handle articles ("the", "a") in parsing

## No-gos

- GUI or graphics
- Sound
- Multiplayer
- Complex inventory management
