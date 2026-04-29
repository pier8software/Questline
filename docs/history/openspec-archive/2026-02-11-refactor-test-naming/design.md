## Context

The testing convention in CLAUDE.md was updated to use plain English, fact-based test naming. The current test suite uses a technical `Action_WhenCondition_ExpectedResult` pattern. This design covers the renaming approach and class reorganisation.

## Goals / Non-Goals

**Goals:**

- Rename all test methods to match the updated convention: declarative statements describing behaviour from a business perspective, with underscores between words
- Split `WorldTests` into focused classes: `WorldBuilderTests`, `PlayerTests`, `GameStateTests`
- Keep test logic, assertions, and setup code identical

**Non-Goals:**

- Changing test behaviour or assertions
- Touching production code
- Restructuring test file locations or namespaces

## Decisions

### Decision 1: Naming pattern

New names follow the CLAUDE.md convention — plain English declarative statements:

| Old Pattern | New Pattern |
|-------------|-------------|
| `GoNorth_WhenExitExists_MovesPlayerToDestination` | `Player_moves_to_destination_when_exit_exists` |
| `Room_HasRequiredProperties` | `Has_required_properties` |
| `Load_ValidAdventure_ConstructsWorld` | `Valid_adventure_constructs_world` |

The class name provides the subject context, so method names don't need to repeat it.

### Decision 2: Split WorldTests

`WorldTests` currently mixes four subjects: WorldBuilder, World, Player, and GameState. Split into:

- `WorldBuilderTests` — builder and exit wiring tests
- `WorldTests` — `GetRoom` lookup tests
- `PlayerTests` — location, inventory tests
- `GameStateTests` — holds world and player

This follows the convention of "class names represent the subject".

### Decision 3: Batch by file

Process one test file at a time, running tests after each to catch regressions immediately.

## Risks / Trade-offs

- **Low risk** — pure rename, no logic changes. If a test fails after rename it indicates an accidental edit, easily caught by the per-file test run.
- **Git blame disruption** — rename touches every test method. Accepted as a one-time cost to align the codebase.
