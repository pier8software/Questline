## Context

The Questline engine has command handlers for all player verbs. Each handler has a corresponding test file in `tests/Questline.Tests/Engine/Handlers/`—except `QuitCommandHandler`. The existing test files follow a consistent pattern using xUnit, Shouldly, and `WorldBuilder` for state setup.

## Goals / Non-Goals

**Goals:**
- Add test coverage for `QuitCommandHandler` matching the project's existing test patterns
- Verify the handler's return type, success flag, and message content

**Non-Goals:**
- Changing the handler implementation
- Testing game loop quit behaviour (that's `GameLoopTests` territory)
- Testing `QuitResult` in isolation (it's a simple record)

## Decisions

### Follow existing handler test conventions

Use the same structure as `LookCommandHandlerTests.cs` and other handler tests: instantiate handler, create a minimal `GameState` via `WorldBuilder`, call `Execute`, assert on the result with Shouldly.

**Alternative considered:** Testing `QuitResult` directly as a unit. Rejected because the project convention is to test handlers, and the handler is what the system actually calls.

## Risks / Trade-offs

- [Minimal risk] The handler is a one-liner, so the tests are simple by nature. But having them ensures regression safety if quit logic ever grows (e.g., save-before-quit).
