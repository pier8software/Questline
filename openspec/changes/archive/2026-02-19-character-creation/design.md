## Context

The game currently has a `Player` entity with `Id`, `Location`, and `Inventory` — but no concept of a named character, race/class, or stats. The `CliApp.Run()` method immediately enters the command loop without any setup flow. Phase 0.5 adds character creation as the first step of a new game, introducing the `Character` model and a `stats` command.

The `Domain/Characters/` bounded context already exists (empty `Entity/` folder), and `Player` lives in `Domain/Players/Entity/`.

## Goals / Non-Goals

**Goals:**

- Introduce `Character` record with name, race, class, level, experience, and stats
- Add name validation as a domain concern
- Add dice rolling abstraction for testable stat generation
- Add `stats` command via the existing command pipeline
- Modify the game start flow to prompt for a name and show a welcome message

**Non-Goals:**

- Race/class selection (hardcoded Human Fighter)
- Stat modifiers or derived attributes
- Combat, XP, or levelling
- Save/load of character state (separate spec)
- UI framework or terminal formatting

## Decisions

### 1. Character as an immutable record on Player

`Character` will be a record with `Name`, `Race`, `Class`, `Level`, `Experience`, and `Stats`. Player gains a `required Character Character { get; init; }` property.

**Why:** Records are the project convention for immutable value types. Character is a value object owned by Player — it has no independent identity. The `with` expression on records supports future mutation (level-up) without breaking immutability.

**Alternative considered:** Separate `Character` entity with its own Id — rejected because at Phase 0 there is always exactly one character per player, and adding identity adds complexity without benefit.

### 2. IDice abstraction in Domain

A `IDice` interface with `int Roll(int sides)` lives in `Domain/Characters/Entity/` (or a shared location). The production implementation uses `Random.Shared`. Tests inject a deterministic fake.

**Why:** Stat generation uses randomness (3d6 per ability, 1d8 for health). Without an abstraction, tests would be non-deterministic. Placing it in Domain keeps the dependency flow clean — Engine and Cli can consume it without inverting dependencies.

**Alternative considered:** Static `Dice` class with a global seed — rejected because it makes parallel test runs fragile and doesn't follow the project's DI patterns.

### 3. CharacterFactory for creation logic

A `CharacterFactory` class accepts `IDice` and a name, performs validation, rolls stats, and returns a `Character`. This keeps creation logic in one testable place.

**Why:** Character creation involves validation + randomness + assembly. A factory method concentrates this. Handlers and CLI code both call the factory rather than duplicating logic.

### 4. Name validation in Domain

Validation rules (2-24 chars, alphanumeric + spaces, no leading/trailing whitespace) live as a static method on `CharacterName` value object or as a validator in `Domain/Characters/`. Invalid names throw or return a result — consistent with the project's `ContentValidationException` pattern.

**Why:** Name validation is a domain rule. Keeping it in Domain ensures the Engine and CLI layers cannot bypass it.

### 5. Stats command via existing command pipeline

`StatsQuery` request + `StatsResponse` response + `StatsQueryHandler` follow the standard mediator pattern. The handler reads `state.Player.Character` and formats the output.

**Why:** This is identical to how every other command works. No new patterns needed.

### 6. Game start flow in CliApp

`CliApp.Run()` will be modified to prompt for a name before entering the game loop. The flow: prompt → read input → validate → create character → set on player → display welcome message → display starting room → enter loop.

**Why:** The CLI layer owns I/O. Character creation is a one-time setup, not a repeatable command, so it belongs in the startup sequence rather than the command pipeline.

**Alternative considered:** A `NewGameCommand` routed through the command pipeline — rejected because the character doesn't exist yet when the pipeline starts, and `GameState` requires a fully constructed `Player`.

## Risks / Trade-offs

- **Player construction timing**: Currently `GameState` is constructed with a `Player` in `CliAppBuilder`. With character creation happening at runtime, `Player` must be constructed after name entry. This requires restructuring how `GameState` is assembled — likely deferring player creation until after the prompt. → Mitigate by having `CliApp.Run()` handle the creation flow and passing the character into a factory method or builder that completes `GameState`.

- **IDice in DI**: Adding `IDice` to DI means `CharacterFactory` is resolved via DI. This is consistent with the project but adds one more registration. → Low risk, standard pattern.

## Open Questions

- Should the welcome message format be defined in content JSON or hardcoded? For Phase 0, hardcoding is simpler and consistent with other messages (e.g., "Goodbye!"). Content-driven messages can be added later.
