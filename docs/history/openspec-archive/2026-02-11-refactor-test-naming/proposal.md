## Why

The CLAUDE.md testing convention was updated to use plain English, fact-based test naming that describes behaviour from a business perspective. The existing 88 test methods across 14 classes use a technical `Action_WhenCondition_ExpectedResult` pattern that doesn't match the new convention. Aligning the test suite now prevents style drift as new tests are added.

## What Changes

- Rename test methods across all 14 test files to use declarative, business-focused names with underscores (e.g. `Player_moves_to_destination_when_exit_exists()`)
- Reorganise test classes where the current grouping mixes concerns (e.g. `WorldTests` contains Player, GameState, and WorldBuilder tests)
- No production code changes — this is a test-only refactor

## Non-goals

- Changing test logic, assertions, or setup code
- Changing production code
- Adding or removing tests
- Modifying specs

## Capabilities

### New Capabilities

_(none)_

### Modified Capabilities

_(none — test naming is not a spec-level concern)_

## Impact

- **Tests only**: All 14 test files under `tests/Questline.Tests/`
- **No production code changes**
- **No spec changes**
- **CI**: All tests must continue to pass with identical behaviour
