# Test Infrastructure

## Purpose

Defines the conventions and contracts for test data construction utilities (builders and templates) used across the test suite.

## Requirements

### Requirement: Test data builders use TestStack.Dossier base class
All entity-level test data builders SHALL extend `TestDataBuilder<T, TBuilder>` from TestStack.Dossier, providing a fluent `Set(x => x.Property, value)` API and a `Build()` method that returns the domain entity.

#### Scenario: Builder creates entity with defaults
- **WHEN** a builder is instantiated and `Build()` is called without any `With*` customisation
- **THEN** it SHALL return a valid domain entity with sensible default values

#### Scenario: Builder allows property override
- **WHEN** a caller chains `.WithName("custom")` (or equivalent `Set` call) before `Build()`
- **THEN** the built entity SHALL have the overridden value

### Requirement: Template classes return pre-configured builders
Template classes (`Items`, `Rooms`, `Barriers`, `Features`) SHALL expose static properties that return *builder instances* (not built objects), allowing further customisation before building.

#### Scenario: Template returns a builder that can be customised
- **WHEN** a test accesses `Templates.Items.BrassLamp.WithDescription("modified")`
- **THEN** the resulting builder SHALL produce an Item with the modified description and all other template defaults

#### Scenario: Template builder produces consistent identity
- **WHEN** a test accesses `Templates.Items.BrassLamp` multiple times
- **THEN** each access SHALL return a fresh builder instance with the same default Id, Name, and Description

### Requirement: GameBuilder accepts RoomBuilder instances
`GameBuilder` SHALL accept `RoomBuilder` instances via a `WithRoom(RoomBuilder)` overload, building the room internally.

#### Scenario: GameBuilder builds fixture from RoomBuilder
- **WHEN** `new GameBuilder().WithRoom(Templates.Rooms.Cellar).Build()` is called
- **THEN** the resulting `GameFixture` SHALL contain a room with the Cellar template defaults

### Requirement: All existing tests pass after refactoring
Refactoring test Arrange sections to use builders and templates SHALL NOT change any test assertions or test behaviour.

#### Scenario: Full test suite passes
- **WHEN** `dotnet test` is executed after the refactoring
- **THEN** all existing tests SHALL pass with no changes to Act or Assert sections
