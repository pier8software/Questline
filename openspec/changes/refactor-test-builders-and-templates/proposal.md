## Why

Test data construction is inconsistent across the test suite. Items, barriers, features, and rooms are built inline with object initialisers, leading to duplicated definitions (e.g. "brass lamp", "iron-door" barrier) scattered across multiple test files. The testing skill defines a Builders + Templates pattern using `TestStack.Dossier` that centralises reusable test data and makes tests more readable, but the existing builders don't follow this pattern and no Template classes exist yet.

## What Changes

- Add `TestStack.Dossier` NuGet package to the test project
- Refactor `RoomBuilder` to extend `TestDataBuilder<Room, RoomBuilder>` from Dossier, replacing the current hand-rolled primary-constructor builder
- Create new Dossier-style builders: `ItemBuilder`, `BarrierBuilder`, `FeatureBuilder`, `PlaythroughBuilder`, `AdventureBuilder`, `ExitBuilder`
- Create Template classes (`Rooms`, `Items`, `Barriers`, `Features`) that return pre-configured builders for common test objects (brass lamp, rusty key, iron door, standard room, cellar, etc.)
- Refactor `GameBuilder` to accept builders/built objects from the new pattern (replacing the string-based `WithRoom` API with builder-accepting overloads)
- Update all handler tests and engine tests to use Templates and the new builders instead of inline object initialisers

## Non-goals

- Changing any production code or domain entities
- Modifying test behaviour or assertions — only the Arrange sections change
- Adding new tests

## Capabilities

### New Capabilities

_None — this is a pure test infrastructure refactoring._

### Modified Capabilities

_None — no spec-level behaviour changes._

## Impact

- **Dependencies**: adds `TestStack.Dossier` NuGet package to `Questline.Tests.csproj`
- **Test helpers**: `tests/Questline.Tests/TestHelpers/Builders/` — all builder files refactored or created
- **Templates**: new `tests/Questline.Tests/TestHelpers/Builders/Templates/` directory with template classes
- **Test files**: all handler test files updated to use templates/builders in Arrange sections
- **No production code changes**
