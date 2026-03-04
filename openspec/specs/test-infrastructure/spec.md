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

### Requirement: Test names use behaviour-as-fact format
All test method names SHALL use plain English, underscore-separated, fact-based naming that describes observable behaviour. Names SHALL NOT begin with `Should`, `Test`, `Verify`, or `Can`.

#### Scenario: Test name describes behaviour
- **WHEN** a test method is named
- **THEN** the name SHALL read as a declarative statement of fact (e.g., `Locked_exit_blocks_player_and_returns_barrier_message`)

#### Scenario: Prefixes are absent
- **WHEN** a test method name is inspected
- **THEN** it SHALL NOT begin with `Should_`, `Test_`, `Verify_`, or `Can_`

### Requirement: Tests assert observable behaviour not internal implementation
Tests SHALL assert on observable outcomes (responses, state changes, side effects) rather than on property assignment or internal method calls. Pure property-bag tests (create object, assert fields equal assigned values) SHALL be removed.

#### Scenario: Property-bag test is removed
- **WHEN** a test only constructs an entity and asserts that its properties match the assigned values
- **THEN** the test SHALL be deleted

#### Scenario: Behavioural assertion is retained
- **WHEN** a test asserts on a state transition, computed result, or system response
- **THEN** the test SHALL be retained

### Requirement: Shared fixture setup lives in class constructor
When multiple test methods in a class share the same Arrange setup, the shared construction SHALL be performed in the class constructor and stored in private readonly fields.

#### Scenario: Constructor initialises shared fixture
- **WHEN** two or more tests in a class use the same `GameBuilder` or handler construction
- **THEN** the construction SHALL appear in the class constructor, not duplicated in each method

### Requirement: Distinct scenarios use nested classes
When tests within a single subject class require different fixture setups (e.g., open exit vs locked barrier), each scenario group SHALL be a `public` nested class with its own constructor.

#### Scenario: Nested class groups related tests
- **WHEN** a test class contains tests with two or more distinct fixture configurations
- **THEN** each configuration SHALL be a nested class named `When_<scenario_description>` or equivalent

### Requirement: Identical-assertion input variations use Theory
When multiple tests share the same assertion logic but differ only in input values, they SHALL be consolidated into a single `[Theory]` with `[InlineData]` attributes.

#### Scenario: Duplicate facts become a theory
- **WHEN** two or more `[Fact]` tests have identical Act and Assert sections differing only in input
- **THEN** they SHALL be replaced by a single `[Theory]` with `[InlineData]` per input variation

### Requirement: Domain entity construction uses builders and templates
Test Arrange sections SHALL construct domain entities (Room, Item, Barrier, Exit, Playthrough) via builder and template classes, not via inline object initialisers or `new Entity { ... }` syntax.

#### Scenario: Inline initialiser replaced with builder
- **WHEN** a test constructs a domain entity using `new Room { Id = ..., Name = ... }`
- **THEN** the construction SHALL be replaced with the appropriate builder (e.g., `new RoomBuilder().WithId(...).WithName(...)`) or a template

### Requirement: All existing tests pass after convention refactoring
Refactoring tests to meet conventions SHALL NOT change any test behaviour. All tests SHALL continue to pass.

#### Scenario: Full test suite passes
- **WHEN** `dotnet test` is executed after convention refactoring
- **THEN** all tests SHALL pass with no changes to production code
