## Context

The test suite has grown organically alongside the game engine. A testing skill (`.claude/skills/testing/SKILL.md`) now codifies conventions for naming, structure, what-to-test, and data construction. The existing `test-infrastructure` spec covers builders and templates but not test conventions themselves. This refactor aligns all ~23 test files with the skill's standards.

## Goals / Non-Goals

**Goals:**
- Remove property-bag tests that only assert assigned values are readable
- Standardise all test names to `<Behaviour_as_fact>` format (no `Should_`/`Test_`/`Verify_` prefixes)
- Hoist shared fixture construction into class constructors
- Use nested classes for distinct scenarios within a single test class
- Convert duplicated `[Fact]` tests into `[Theory]` where applicable
- Ensure all domain entity construction uses builders/templates, not inline initialisers

**Non-Goals:**
- Adding new test coverage or new test files
- Modifying production code
- Changing the builder/template infrastructure
- Refactoring test helpers (FakeConsole, FakeDice, etc.)

## Decisions

### Decision 1: Remove property-bag tests entirely rather than converting them

Property-bag tests in `BarrierTests`, `RoomTests`, and parts of `CharacterTests` / `PlaythroughTests` assert that setting a property and reading it back returns the same value. These test the C# compiler, not behaviour.

**Alternative considered**: Convert to behavioural tests. Rejected because these entities already have behavioural coverage in handler tests (e.g., `MovePlayerCommandHandlerTests` exercises `Barrier` locking behaviour, `GetRoomDetailsHandlerTests` exercises `Room` rendering).

**Approach**: Delete pure property-bag tests. Keep tests that verify computed behaviour (e.g., `PlaythroughTests.MoveTo_updates_location` is behavioural — it tests state transition logic).

### Decision 2: Constructor setup for shared fixtures, nested classes for scenario variation

The skill prescribes constructor-level setup for shared fixtures. Where a test class has a single scenario context, move the `GameBuilder` setup into the constructor. Where a class needs multiple distinct setups (e.g., `MovePlayerCommandHandlerTests` with open exits vs locked barriers), use `public nested class` with its own constructor.

**Alternative considered**: Static factory methods per scenario. Rejected because nested classes give better xUnit test explorer grouping and match the skill's explicit guidance.

### Decision 3: Incremental file-by-file refactoring

Each test file is refactored independently. No cross-file dependencies change. This keeps each step small and verifiable with `dotnet test`.

## Risks / Trade-offs

- **[Removing tests reduces coverage]** → Mitigated: only removing tests that verify no behaviour. Handler-level tests already cover these entities in context.
- **[Nested class refactoring may miss assertions]** → Mitigated: run `dotnet test` after each file change to catch regressions immediately.
- **[Large number of files to touch]** → Mitigated: changes are mechanical and can be validated incrementally. Files that already comply are skipped.
