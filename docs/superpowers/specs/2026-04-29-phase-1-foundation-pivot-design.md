# Phase 1 — Foundation Pivot Design

**Date:** 2026-04-29
**Status:** Approved (brainstorming → design); awaiting implementation plan
**Phase:** 1 of 6 (see [`docs/roadmap.md`](../../roadmap.md))

## Context

Questline began as a parser-driven text adventure intended to evolve into a cooperative MUD platform. The project is now redirected toward a digital OSR (Old School Renaissance) party-based RPG in the spirit of Old School Essentials and Dungeon Crawl Classics, with optional co-op as a long-term feature. The full direction and phasing live in [`docs/roadmap.md`](../../roadmap.md); the architectural overview that this design extends lives in [`docs/architecture.md`](../../architecture.md).

A design review of the existing codebase identified four mismatches with the new direction:

- A single `Character` per `Playthrough`, where the new direction needs a fixed party of four.
- D&D-flavoured stats (`Race`, `Class`, `AbilityScores`, `HitPoints`) exist on `Character` but are not actually used by any handler. They are dead code that signals an RPG the engine isn't.
- The parser handles a single anonymous actor (the player). Co-op (Phase 6) and combat (Phase 2) both require attribution of commands to a specific character.
- No turn counter, no live stats, and no generic dice / check primitive — all of which later phases need.

Phase 1's goal is to put the foundation in place so all of the above work can land *additively* in later phases without invasive refactors.

## Goals & Scope

### In scope

- A `Party` of four `Character`s as the unit of play, replacing the single-character `Playthrough`.
- A level-0 character model: race, ability scores, HP, flavour occupation. **No class.**
- A funnel-style party creation flow replacing the current single-character creation flow.
- **Action attribution**: parser distinguishes party-level commands from per-PC commands; handlers receive an `Actor`.
- **Live stats**: HP can be mutated; ability score modifiers are queryable.
- **Generic dice / check primitive** (`Roll`, `Check(modifier, dc)`) on the existing `IDice` abstraction.
- **Turn counter**, ticking once per accepted command in the `Playing` phase.

### Explicitly out of scope (deferred to later phases)

- Combat, monsters, attacks, damage rolls, saves applied to PCs.
- Spells, spell preparation, class features.
- Equipment slots, containers, encumbrance, light burning down per turn.
- Class selection, level-up flow, XP awards.
- Random tables, encounter checks, reaction rolls, story scripting.
- Co-op / multi-client sessions.
- DCC occupation starter equipment.

### Success criteria

- The existing adventure (`the-goblins-lair.json`) is playable end-to-end with a four-PC party.
- All current verbs (`look`, `go`, `take`, `drop`, `examine`, `inventory`, `use`, `quit`, `version`) work as party-level commands by default.
- An actor prefix on a verb (e.g. `aric examine altar`) routes the command to that PC, and the response message reflects it.
- The turn counter is observable via the `stats` command output.
- Party creation is deterministic under a stub `IDice`.
- All tests are green.

## Domain Model

### Entities

**`Character`** — already exists; modified.

| Field | Type | Notes |
|---|---|---|
| `Id` | `string` | Existing. |
| `Name` | `string` | Existing. Validated unique within party, no reserved-verb collisions. |
| `Race` | `Race` | Existing. |
| `Class` | `Class?` | **Now nullable.** Always `null` at level 0. |
| `Level` | `int` | Existing. Defaults to `0`. |
| `Experience` | `int` | Existing. |
| `AbilityScores` | `AbilityScores` | Existing. |
| `HitPoints` | `HitPoints` | Existing. **Now mutable** (see [Live Stats](#live-stats--dice-abstraction)). |
| `Occupation` | `string` | **New.** Flavour string only in Phase 1 (e.g. `"Beekeeper"`). Starter equipment lands in Phase 3. |
| `Inventory` | `List<Item>` | **Moved here from `Playthrough`.** Per-PC inventory. |

**`Party`** — new entity.

| Field | Type | Notes |
|---|---|---|
| `Id` | `string` | New entity identity. |
| `Members` | `IReadOnlyList<Character>` | Ordered. Index 0 is the front of the marching order. |

Methods:

- `FindByName(string name) → Character?` — case-insensitive lookup.
- `IsMember(Character character)`.
- `MembersAlive` — projection over `Members.Where(c => c.HitPoints.IsAlive)`. Useful in Phase 2; cheap to add now.

**`Playthrough`** — existing; refactored.

| Field | Status | Notes |
|---|---|---|
| `Id` | unchanged | |
| `Username` | unchanged | |
| `AdventureId` | unchanged | |
| `StartingRoomId` | unchanged | |
| `Location` | unchanged | Current room ID. |
| `UnlockedBarriers` | unchanged | |
| `RoomItems` | unchanged | Per-playthrough dynamic room state. |
| `Party` | **new** | Replaces denormalised character fields. |
| `Turns` | **new** | `int`, default `0`. |
| `CharacterName`, `Race`, `Class`, `Level`, `Experience`, `AbilityScores`, `HitPoints`, `Inventory` | **removed** | All now on `Character` inside `Party`. |

**`Player`** — unchanged. Still `Username` + `Name`.

### Folder layout

Following the existing bounded-context convention:

```
src/Questline/Domain/
  Parties/                  ← new bounded context
    Entity/
      Party.cs
  Characters/               ← existing
    Entity/
      Character.cs          ← Class nullable, Inventory added, Occupation added
  Playthroughs/             ← existing
    Entity/
      Playthrough.cs        ← references Party, drops denormalised fields
```

### Persistence

`PlaythroughDocument` flattens the `Party` and its `Character`s as nested documents. No new MongoDB collection is required.

```csharp
class PlaythroughDocument : Document
{
    public string Username { get; set; }
    public string AdventureId { get; set; }
    public string StartingRoomId { get; set; }
    public string Location { get; set; }
    public int Turns { get; set; }
    public PartyDocument Party { get; set; }
    public List<string> UnlockedBarriers { get; set; }
    public Dictionary<string, List<ItemDocument>> RoomItems { get; set; }
}

class PartyDocument
{
    public List<CharacterDocument> Members { get; set; }
}

class CharacterDocument
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Race { get; set; }
    public string? Class { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public string Occupation { get; set; }
    public AbilityScoresDocument AbilityScores { get; set; }
    public HitPointsDocument HitPoints { get; set; }
    public List<ItemDocument> Inventory { get; set; }
}
```

### Migration

Existing saved playthroughs are dev-only at this stage (Aspire-managed Mongo, no production deployments). The recommended approach is **drop-and-reseed**: drop the `Playthroughs` and `Adventures` collections on first run after the change, then re-seed via the existing `deploy-content` mode. No migration script is written for Phase 1. If real players exist by Phase 2, migration is revisited then.

## Action Attribution

The parser learns to recognise an optional actor prefix.

### Parser rule

1. Tokenise input as today (whitespace split, lower-case).
2. If the first token case-insensitively matches a PC name in the current party, that PC is the **actor**; drop the token, continue parsing the rest as today.
3. Otherwise, the actor is the **party** (`PartyActor`).

A request marked `[RequiresActor]` (see below) parsed without an actor prefix produces a parse failure response asking which character should act, and does not dispatch.

### Validation on PC names

Performed at party creation:

- Unique within the party (case-insensitive).
- Not equal to any reserved verb — the union of every `[Verbs(...)]` value across `IRequest` types.
- Single token (no whitespace).

If the random name generator produces a colliding name, that single PC's name is re-rolled; the rest of the party is unaffected.

### Handler signature

```csharp
public interface IRequestHandler<TRequest> where TRequest : IRequest
{
    Task<IResponse> Handle(GameState state, Actor actor, TRequest request);
}
```

`Actor` is a sealed discriminated union:

```csharp
public abstract record Actor;
public sealed record PartyActor(Party Party) : Actor;
public sealed record CharacterActor(Character Character) : Actor;
```

In Phase 1 most handlers do not behaviourally distinguish the two cases. Where they do, it is response-message wording only: a command routed to a `CharacterActor` produces a third-person response naming the character (`"Aric picks up the rusty key."`), while the same command routed to a `PartyActor` produces a second-person response (`"You pick up the rusty key."`).

### `[RequiresActor]` attribute

A new attribute marks requests that have no party-level meaning:

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class RequiresActorAttribute : Attribute { }
```

**No request carries the attribute in Phase 1** — combat verbs land in Phase 2. The attribute itself is defined, the parser honours it (raising the actor-required failure response when missing), and tests against a fake request exercise the path.

### `RequestSender` change

`RequestSender.Send` gains an `Actor` parameter and forwards it to the resolved handler. Reflection-based dispatch is otherwise unchanged.

## Live Stats & Dice Abstraction

### Mutable `HitPoints`

```csharp
public class HitPoints
{
    public int Max { get; private init; }
    public int Current { get; private set; }

    public void Damage(int amount);   // clamped at 0
    public void Heal(int amount);     // clamped at Max
    public bool IsAlive => Current > 0;
}
```

Tests verify clamping. No production code calls `Damage` in Phase 1; `Heal` is unused too. Both methods exist so Phase 2 combat can use them without revisiting the type.

### `AbilityScore.Modifier`

```csharp
public record AbilityScore(int Score)
{
    public int Modifier => (Score - 10) / 2;
}
```

A standard D&D-flavoured modifier. OSR-specific modifier tables differ slightly; if Phase 4 commits to OSE the modifier may switch to a table lookup, but call sites are unchanged.

### `IDice` extension

The existing `IDice` abstraction is extended:

```csharp
public interface IDice
{
    int Roll(int sides);              // 1d20 → Roll(20)
    int Roll(int count, int sides);   // 3d6 → Roll(3, 6)
}

public static class DiceExtensions
{
    public static CheckResult Check(this IDice dice, int modifier, int dc)
    {
        var roll = dice.Roll(20);
        return new CheckResult(roll, modifier, dc, roll + modifier >= dc);
    }
}

public record CheckResult(int Roll, int Modifier, int DC, bool Success);
```

In Phase 1 no production code calls `Check`. Tests verify it against a stubbed `IDice`. `FakeDice` is updated to return scripted values for both `Roll(sides)` and `Roll(count, sides)`.

## Turn Counter

`Playthrough.Turns` increments by one inside `GameEngine.HandleGamePlay()` after a command both parses and dispatches successfully. No increment on:

- Parse failures.
- Commands handled outside the `Playing` phase.
- Inputs handled by the engine state machine itself (e.g. `accept` during party creation).

The `stats` command response gains a `Turns: <n>` line. No other surface in Phase 1.

In Phase 3 the same counter ticks light-burn / encumbrance / wandering-monster checks; its meaning expands without a structural change.

## Party Creation Flow

The existing `CharacterCreationStateMachine` is removed and replaced with `PartyCreationStateMachine`. The engine phase `CharacterCreation` is renamed to `PartyCreation`.

### Phase transitions

```
Started → Login → StartMenu → (NewAdventure → PartyCreation | LoadGame) → Playing → Ended
```

### Flow

1. **On entry**, the engine rolls a 4-PC party using `IDice`:
   - **Race**: random from `{Human, Dwarf, Elf, Halfling}`.
   - **Ability scores**: 3d6 in order, six times.
   - **HP**: 1d4 (level-0 baseline).
   - **Occupation**: random from a Phase-1 list of approximately twenty entries (beekeeper, miller, scribe, woodcutter, herder, etc.).
   - **Name**: random from a per-race name table.
   - **Validation**: names unique within the party and not colliding with reserved verbs; collision triggers a re-roll of just that name.
2. The engine displays the four characters in a single response (name, race, occupation, six ability scores with modifiers, HP).
3. The engine accepts one of:
   - `accept` — proceed to `Playing`.
   - `reroll` — re-roll the entire party (returns to step 1).
   - `rename <slot> <newName>` — rename PC in slot 1–4.
   - `rename <oldName> <newName>` — rename by current name.
4. Repeat steps 2–3 until `accept`.

### Persistence

Only on `accept` is the `Playthrough` created and saved with the new `Party`. The party is stable for the rest of the playthrough in Phase 1; death and recruit land in Phase 2.

### Phase-1 occupation list

Illustrative — the final list is set during implementation:

`Apprentice scribe`, `Beekeeper`, `Charcoal-burner`, `Cooper`, `Cordwainer`, `Dyer`, `Falconer's hand`, `Fishmonger`, `Goose-girl`, `Herbalist`, `Hireling porter`, `Inn-keeper`, `Lamplighter`, `Mason's apprentice`, `Miller`, `Outlaw`, `Pilgrim`, `Smithy boy`, `Tinker`, `Wandering minstrel`.

## CLI / Player-facing changes

- `stats` output is rewritten to list all four PCs (name, race, occupation, ability scores with modifiers, HP) and shows `Turns: <n>`.
- Per-PC response wording: a command routed to a `CharacterActor` produces a third-person response naming the character; a `PartyActor` command keeps the existing second-person wording.
- The single-character creation prompts (class selection, race selection, name) are removed.
- The `Welcome, <character name>` greeting becomes a party-introduction message.

## Test Strategy

Following project convention (`<Subject>/When_<scenario>.cs`):

**New tests:**

- `Domain/Parties/Entity/Party/When_finding_member_by_name.cs`
- `Domain/Parties/Entity/Party/When_party_has_no_living_members.cs`
- `Domain/Characters/Entity/HitPoints/When_taking_damage.cs`
- `Domain/Characters/Entity/HitPoints/When_healing.cs`
- `Domain/Characters/Entity/AbilityScore/When_calculating_modifier.cs`
- `Engine/Parsers/Parser/When_input_starts_with_actor_name.cs`
- `Engine/Parsers/Parser/When_input_has_no_actor_prefix.cs`
- `Engine/Parsers/Parser/When_required_actor_is_missing.cs`
- `Engine/Characters/PartyCreationStateMachine/When_rolling_initial_party.cs`
- `Engine/Characters/PartyCreationStateMachine/When_rerolling.cs`
- `Engine/Characters/PartyCreationStateMachine/When_renaming_a_character_by_slot.cs`
- `Engine/Characters/PartyCreationStateMachine/When_renaming_a_character_by_name.cs`
- `Framework/Dice/DiceExtensions/When_check_succeeds.cs`
- `Framework/Dice/DiceExtensions/When_check_fails.cs`

**Tests removed:**

- `Engine/Characters/CharacterCreationStateMachine/*` — replaced by `PartyCreationStateMachine` tests.

**Tests updated:**

- All existing handler tests gain an `Actor` argument and verify that a `CharacterActor` produces an actor-flavoured response.
- `Domain/Playthroughs/Entity/Playthrough/*` — tests for inventory, barriers, room items continue to work but now go through a `Party` member.

`FakeDice` is updated to support deterministic multi-die rolls. `GameBuilder` and `PlaythroughBuilder` gain methods to seed a party with specified PCs.

## Deferred Decisions

Decisions explicitly **not** made in Phase 1. Future sessions opening this design doc cold should know what is still open and where it lands.

- **OSE vs DCC ruleset.** Foundation is system-agnostic. Decision lands in Phase 4 (class features & magic).
- **Race-as-class flattening.** Race is a tag at level 0; class is `null`. Flattening (or not) is a Phase 4 decision when classes appear.
- **Magic-User vs Wizard naming.** Tied to the OSE-vs-DCC choice. Phase 4.
- **Saving throws.** First needed for combat. Phase 2.
- **AC, attack rolls, damage application.** Phase 2.
- **Containers, equipment slots, encumbrance, light & darkness.** Phase 3.
- **Predicate-based gating** (replacing the over-specific `Barrier`). Phase 5.
- **Story scripting / event-rule system.** Phase 5.
- **DCC occupation starter equipment.** Phase 3 (in Phase 1, occupations are flavour strings only).
- **Co-op multi-client session.** Phase 6.
- **Save-game migration.** Drop-and-reseed for Phase 1; revisit if real players exist by Phase 2.
- **Hirelings, retainers, NPC companions.** Likely Phase 5 with the random-table system.

## Future Phases

Brief overview; [`docs/roadmap.md`](../../roadmap.md) is the canonical version.

- **Phase 2 — Combat core.** Encounters, initiative, attack/AC, damage, saves, monster entities, parser additions for `attack` / `cast`. Death becomes real; the funnel becomes a real funnel.
- **Phase 3 — Equipment & dungeon procedure.** Item state, equipment slots, containers, encumbrance, light burning per turn, dungeon turn clock. DCC occupation starter equipment.
- **Phase 4 — Class features & Vancian magic.** Spell prep & cast, thief skills, cleric turning, warrior abilities. Class selection on funnel survival. **Pick OSE or DCC here.**
- **Phase 5 — Adventure scripting & random tables.** Event/rule system, predicate-based gating, random tables, wandering monsters, reaction rolls, level-up flow.
- **Phase 6 — Co-op.** Multi-client session, character ownership per connected human.

XP and levelling thread through Phases 2–5; parser sophistication progresses as each phase needs it.

## How future sessions pick this up

1. Read [`docs/roadmap.md`](../../roadmap.md) to confirm current phase status.
2. Read this design doc's [Deferred Decisions](#deferred-decisions) section to see what is still open.
3. Read the implementation plan for the phase being started (separate document, produced by the writing-plans skill).
4. Git history of the previous phase's implementation provides concrete context.
