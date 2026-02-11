## Why

Room exits are currently modeled as `Dictionary<Direction, string>` — a direction mapped to a bare destination room ID. The upcoming puzzles-and-barriers capability (Phase 0.4) requires exits to carry additional data (barrier references). Refactoring the exit model now, as a standalone change, isolates the structural ripple from the feature work and keeps both changes reviewable.

## What Changes

- **BREAKING**: Introduce an `Exit` value object in `Questline.Domain` with `Destination` (required) and `BarrierId` (optional, null by default)
- **BREAKING**: Change `Room.Exits` from `Dictionary<Direction, string>` to `Dictionary<Direction, Exit>`
- Update `WorldBuilder` / `RoomBuilder` to construct `Exit` objects (preserve ergonomic API for simple exits)
- Update `GoCommandHandler` and `LookCommandHandler` to read `Exit.Destination` instead of the raw string
- Update `FileSystemAdventureLoader` to map `ExitDto` → `Exit` (the DTO layer already supports the barrier field; this change threads it into the domain)
- Update all existing tests to use the new `Exit` type

## Non-goals

- Adding barrier logic, the `use` command, or the `examine` command — those belong to the puzzles-and-barriers change
- Changing the JSON content format — the existing `ExitDto` / `ExitDictionaryConverter` already handles both simple and object-form exits
- Populating `BarrierId` from content — this change introduces the field but barrier loading is deferred

## Capabilities

### New Capabilities

_(none — this is a refactor of existing internals)_

### Modified Capabilities

- `world-model`: The exit representation changes from a plain destination string to an `Exit` value object carrying `Destination` and optional `BarrierId`

## Impact

- **Domain**: `Room.cs` exit type changes; new `Exit.cs` file
- **Engine**: `GoCommandHandler`, `LookCommandHandler` updated to use `Exit.Destination`
- **Framework**: `FileSystemAdventureLoader` maps `ExitDto` → `Exit`
- **Tests**: All tests that build worlds via `WorldBuilder` or assert on exits need updating
- **No user-facing behaviour change** — all existing commands produce identical output
