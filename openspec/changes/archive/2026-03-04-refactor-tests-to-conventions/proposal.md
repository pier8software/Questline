## Why

The testing skill (`.claude/skills/testing/SKILL.md`) establishes conventions for test naming, structure, assertions, and what to test. The current test suite predates these conventions and has inconsistencies: property-bag tests that verify nothing behavioural, missing constructor-level shared setup, inconsistent naming (some use `Should`/`Test` prefixes), and tests that don't leverage the builder/template infrastructure fully.

## What Changes

- **Remove property-bag tests** in `BarrierTests`, `RoomTests`, `CharacterTests`, and `PlaythroughTests` that only assert assigned properties are readable — replace with behavioural tests or remove entirely
- **Standardise test naming** to `<Behaviour_as_fact>` format with underscores; remove `Should_`/`Test_`/`Verify_` prefixes
- **Move shared setup to constructors** where multiple tests in a class share the same fixture construction
- **Use nested classes** for distinct scenarios that need different setup (per the skill's structure guidance)
- **Convert repeated [Fact] tests to [Theory]** where inputs vary but assertions are identical
- **Ensure all handler tests use builders and templates** consistently (no inline object initialisers for domain entities)

## Non-goals

- Adding new test coverage — this is a refactor of existing tests only
- Changing production code — only test files are modified
- Migrating to a different test framework or assertion library
- Modifying the builder/template infrastructure itself (already compliant with `test-infrastructure` spec)

## Capabilities

### New Capabilities

_None — this change refactors existing tests, it does not introduce new capabilities._

### Modified Capabilities

- `test-infrastructure`: Adding conventions for test structure, naming, and what-to-test guidance alongside existing builder/template requirements

## Impact

- **Code**: All files under `tests/Questline.Tests/` — approximately 23 test files
- **Risk**: Low — only test code changes; production code untouched. All tests must continue to pass.
- **Dependencies**: None — no new packages or tools
