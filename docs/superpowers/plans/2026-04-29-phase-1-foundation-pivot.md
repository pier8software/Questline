# Phase 1 — Foundation Pivot Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace single-character `Playthrough` with a four-PC `Party`, introduce action attribution (party vs. character), make HP/ability stats live, add a generic dice/check primitive, add a turn counter, and replace single-character creation with a DCC-style level-0 funnel party creation flow.

**Architecture:** Tight refactor that touches Domain (new `Party` entity, mutable `HitPoints`, `Character` extensions), Engine (Parser actor-prefix detection, handler signatures gain `Actor`, new `PartyCreationStateMachine`), Framework (mediator dispatch passes `Actor`), and persistence (denormalised character fields on `PlaythroughDocument` collapse into a `PartyDocument`). Existing single-player flow continues to work end-to-end after each tier.

**Tech Stack:** .NET 10, xUnit + Shouldly, MongoDB (via Aspire), Mediator-style request/handler pipeline.

**Spec:** [`docs/superpowers/specs/2026-04-29-phase-1-foundation-pivot-design.md`](../specs/2026-04-29-phase-1-foundation-pivot-design.md)

---

## Notes for the implementer

**Two small clarifications versus the spec, decided during planning:**

1. **`Actor` adds a third variant `NoActor`.** The spec showed only `PartyActor` and `CharacterActor`. The login and version commands are dispatched through the same handler pipeline but have no party context (login happens before any playthrough exists). Rather than make `Actor` nullable everywhere, the plan introduces `NoActor` as a sentinel passed for system-level commands. `PartyActor` and `CharacterActor` retain their roles for in-Playing-phase commands.

2. **`PartyActor` carries no fields.** The spec showed `PartyActor(Party Party)`. Handlers always have access to the playthrough (and therefore the party) via injected services, so the `Party` field on `PartyActor` is redundant. The cleaner shape is `public sealed record PartyActor : Actor;`. `CharacterActor(Character Character)` keeps its field.

3. **Handler signature gains `Actor` only — not `(GameState, Actor, TRequest)` as the spec showed.** The current handler signature is `Handle(TRequest)` (no `GameState` parameter — handlers fetch state via injected repositories). The plan is faithful to current code: `Handle(Actor, TRequest)`.

**Test convention:** Subject-folder + `When_<scenario>.cs` containing `public class When_<scenario>` with plain-English fact methods. See `CLAUDE.md` § Test Conventions.

**Build & test commands:**

- Build: `dotnet build`
- Test: `dotnet test --no-build`
- Run game: `dotnet run --project src/Questline`
- Dev environment: `aspire run` (required before integration tests / running the game)

**Branching:** Create a feature branch `feature/phase-1-foundation-pivot` at the start. Each task ends with a commit. Squash-merge via PR at the end (per project convention).

---

## Pre-flight

### Task 0: Create feature branch

- [ ] **Step 0.1: Create and check out the branch**

```bash
git checkout -b feature/phase-1-foundation-pivot
```

- [ ] **Step 0.2: Verify build is green before starting**

```bash
dotnet build && dotnet test --no-build
```

Expected: build succeeds, all tests pass.

---

## Tier 1 — Domain primitives

### Task 1: Make `HitPoints` mutable

`HitPoints` is currently a record: `public record HitPoints(int MaxHitPoints, int CurrentHitPoints);`. We change it to a class with `Damage`, `Heal`, and `IsAlive`. The persistence mapper and `Playthrough` must continue to work.

**Files:**

- Modify: `src/Questline/Domain/Characters/Entity/HitPoints.cs`
- Modify: `src/Questline/Engine/Persistence/Playthroughs/PlaythroughMapper.cs` (constructor call)
- Create: `tests/Questline.Tests/Domain/Characters/Entity/HitPoints/When_taking_damage.cs`
- Create: `tests/Questline.Tests/Domain/Characters/Entity/HitPoints/When_healing.cs`

- [ ] **Step 1.1: Write `When_taking_damage` tests**

Create `tests/Questline.Tests/Domain/Characters/Entity/HitPoints/When_taking_damage.cs`:

```csharp
using Questline.Domain.Characters.Entity;
using Shouldly;

namespace Questline.Tests.Domain.Characters.Entity.HitPoints;

public class When_taking_damage
{
    [Fact]
    public void Current_decreases_by_damage_amount()
    {
        var hp = new Questline.Domain.Characters.Entity.HitPoints(max: 8, current: 8);

        hp.Damage(3);

        hp.Current.ShouldBe(5);
        hp.Max.ShouldBe(8);
    }

    [Fact]
    public void Current_clamps_at_zero_when_damage_exceeds_current()
    {
        var hp = new Questline.Domain.Characters.Entity.HitPoints(max: 8, current: 4);

        hp.Damage(10);

        hp.Current.ShouldBe(0);
    }

    [Fact]
    public void Is_alive_when_current_is_positive()
    {
        var hp = new Questline.Domain.Characters.Entity.HitPoints(max: 8, current: 1);

        hp.IsAlive.ShouldBeTrue();
    }

    [Fact]
    public void Is_not_alive_when_current_is_zero()
    {
        var hp = new Questline.Domain.Characters.Entity.HitPoints(max: 8, current: 0);

        hp.IsAlive.ShouldBeFalse();
    }
}
```

- [ ] **Step 1.2: Write `When_healing` tests**

Create `tests/Questline.Tests/Domain/Characters/Entity/HitPoints/When_healing.cs`:

```csharp
using Questline.Domain.Characters.Entity;
using Shouldly;

namespace Questline.Tests.Domain.Characters.Entity.HitPoints;

public class When_healing
{
    [Fact]
    public void Current_increases_by_heal_amount()
    {
        var hp = new Questline.Domain.Characters.Entity.HitPoints(max: 8, current: 3);

        hp.Heal(2);

        hp.Current.ShouldBe(5);
    }

    [Fact]
    public void Current_clamps_at_max_when_heal_would_exceed_max()
    {
        var hp = new Questline.Domain.Characters.Entity.HitPoints(max: 8, current: 6);

        hp.Heal(10);

        hp.Current.ShouldBe(8);
    }
}
```

- [ ] **Step 1.3: Run tests; expect failure**

```bash
dotnet test --no-build --filter "FullyQualifiedName~Domain.Characters.Entity.HitPoints"
```

Expected: compile error or `Damage`/`Heal`/`IsAlive`/`Current`/`Max` not found on `HitPoints`.

- [ ] **Step 1.4: Replace `HitPoints` with a mutable class**

Replace the contents of `src/Questline/Domain/Characters/Entity/HitPoints.cs` with:

```csharp
namespace Questline.Domain.Characters.Entity;

public class HitPoints
{
    public HitPoints(int max, int current)
    {
        Max     = max;
        Current = current;
    }

    public int Max     { get; }
    public int Current { get; private set; }

    public bool IsAlive => Current > 0;

    public void Damage(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Damage amount must be non-negative.");
        }

        Current = Math.Max(0, Current - amount);
    }

    public void Heal(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Heal amount must be non-negative.");
        }

        Current = Math.Min(Max, Current + amount);
    }
}

public record AbilityScores(
    AbilityScore Strength,
    AbilityScore Intelligence,
    AbilityScore Wisdom,
    AbilityScore Dexterity,
    AbilityScore Constitution,
    AbilityScore Charisma);

public record AbilityScore(int Score);
```

Note: the existing file currently bundles `AbilityScores`/`AbilityScore` in the same file. Keep them here — Task 2 modifies `AbilityScore` separately.

- [ ] **Step 1.5: Update `PlaythroughMapper` to use new `HitPoints` constructor**

`HitPoints` no longer has positional record syntax. Update both directions of the mapper. In `src/Questline/Engine/Persistence/Playthroughs/PlaythroughMapper.cs`:

Replace:
```csharp
HitPoints = new HitPoints(
    document.HitPoints.MaxHitPoints,
    document.HitPoints.CurrentHitPoints),
```

With:
```csharp
HitPoints = new HitPoints(
    max:     document.HitPoints.MaxHitPoints,
    current: document.HitPoints.CurrentHitPoints),
```

The reverse direction (entity → document) already uses property reads (`entity.HitPoints.MaxHitPoints` / `CurrentHitPoints`). Update those to `entity.HitPoints.Max` / `entity.HitPoints.Current`:

Replace:
```csharp
HitPoints = new HitPointsDocument
{
    MaxHitPoints     = entity.HitPoints.MaxHitPoints,
    CurrentHitPoints = entity.HitPoints.CurrentHitPoints
},
```

With:
```csharp
HitPoints = new HitPointsDocument
{
    MaxHitPoints     = entity.HitPoints.Max,
    CurrentHitPoints = entity.HitPoints.Current
},
```

- [ ] **Step 1.6: Update other call sites that read `MaxHitPoints` / `CurrentHitPoints`**

Find and update any remaining call sites by running:

```bash
git grep -nE "MaxHitPoints|CurrentHitPoints" src/ tests/
```

Update each from `MaxHitPoints` → `Max` and `CurrentHitPoints` → `Current` on a `HitPoints` entity reference. Known sites:

- `src/Questline/Domain/Characters/Entity/Character.cs` `ToSummary()` method (`HitPoints.MaxHitPoints` → `HitPoints.Max`, `HitPoints.CurrentHitPoints` → `HitPoints.Current`).
- `src/Questline/Domain/Playthroughs/Entity/Playthrough.cs` `ToCharacterSummary()` method (same renames).
- Any test that constructs `new HitPoints(8, 8)` — switch to `new HitPoints(max: 8, current: 8)` (positional still works but named is clearer).
- `src/Questline/Domain/Characters/HitPointsCalculator.cs` (if it exists) — verify return constructor.

Also: check `src/Questline/Domain/Characters/Data/CharacterSummary.cs` — its property names (e.g. `MaxHitPoints`) are part of the summary record's external surface and may stay unchanged, but the call sites that *populate* them must read from `HitPoints.Max` / `HitPoints.Current`.

- [ ] **Step 1.7: Run tests; expect pass**

```bash
dotnet build && dotnet test --no-build
```

Expected: all tests green, including the two new files.

- [ ] **Step 1.8: Commit**

```bash
git add src/ tests/
git commit -m "Make HitPoints mutable with Damage/Heal/IsAlive

Replace the immutable HitPoints record with a class that has Max,
Current, IsAlive, Damage, and Heal. Update PlaythroughMapper and
ToSummary callers to use the new property names.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

### Task 2: Add `AbilityScore.Modifier`

D&D-style modifier: `(Score - 10) / 2` (integer division floors toward zero).

**Files:**

- Modify: `src/Questline/Domain/Characters/Entity/HitPoints.cs` (the file that currently holds `AbilityScore`)
- Create: `tests/Questline.Tests/Domain/Characters/Entity/AbilityScore/When_calculating_modifier.cs`

- [ ] **Step 2.1: Write the failing test**

Create `tests/Questline.Tests/Domain/Characters/Entity/AbilityScore/When_calculating_modifier.cs`:

```csharp
using Questline.Domain.Characters.Entity;
using Shouldly;

namespace Questline.Tests.Domain.Characters.Entity.AbilityScore;

public class When_calculating_modifier
{
    [Theory]
    [InlineData(3, -4)]
    [InlineData(8, -1)]
    [InlineData(9, -1)]
    [InlineData(10, 0)]
    [InlineData(11, 0)]
    [InlineData(12, 1)]
    [InlineData(13, 1)]
    [InlineData(14, 2)]
    [InlineData(18, 4)]
    public void Modifier_matches_dnd_formula(int score, int expectedModifier)
    {
        var ability = new Questline.Domain.Characters.Entity.AbilityScore(score);

        ability.Modifier.ShouldBe(expectedModifier);
    }
}
```

Note: integer division of `(score - 10)` by `2` gives the right answer for non-negative values but rounds **toward zero** for negative values (e.g., `(8 - 10) / 2 = -1`, not `-1.0`). The existing C# `/` behaviour is correct for D&D modifiers without extra logic — the test covers a few negatives to verify.

- [ ] **Step 2.2: Run test; expect failure**

```bash
dotnet test --no-build --filter "FullyQualifiedName~AbilityScore.When_calculating_modifier"
```

Expected: compile error — `Modifier` not found on `AbilityScore`.

- [ ] **Step 2.3: Implement `Modifier`**

In `src/Questline/Domain/Characters/Entity/HitPoints.cs`, change `AbilityScore` from:

```csharp
public record AbilityScore(int Score);
```

to:

```csharp
public record AbilityScore(int Score)
{
    public int Modifier => (Score - 10) / 2;
}
```

- [ ] **Step 2.4: Run test; expect pass**

```bash
dotnet test --no-build --filter "FullyQualifiedName~AbilityScore.When_calculating_modifier"
```

Expected: PASS.

- [ ] **Step 2.5: Commit**

```bash
git add src/ tests/
git commit -m "Add AbilityScore.Modifier (D&D-flavoured)

(Score - 10) / 2. Used by future skill checks and combat in Phase 2+.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

### Task 3: Redesign `IDice` with scalar overloads

Current: `int[] Roll(int diceAmount, int sides)` returning an array. New: two methods returning `int`. The change cascades to `FakeDice`, `Dice`, `AbilityScoresCalculator`, and `HitPointsCalculator`.

**Files:**

- Modify: `src/Questline/Engine/Characters/IDice.cs`
- Modify: `tests/Questline.Tests/TestHelpers/FakeDice.cs`
- Modify: `src/Questline/Domain/Characters/AbilityScoresCalculator.cs`
- Modify: `src/Questline/Domain/Characters/HitPointsCalculator.cs` (if exists; otherwise wherever HP is rolled)

- [ ] **Step 3.1: Read current `AbilityScoresCalculator` and `HitPointsCalculator`**

```bash
cat src/Questline/Domain/Characters/AbilityScoresCalculator.cs
```

Then check if `HitPointsCalculator` exists:

```bash
git grep -nE "HitPointsCalculator|class.*HitPoints" src/Questline/Domain/Characters/
```

The implementer needs to see the current calculation logic to update it. Both currently use the array-returning `Roll` and likely sum or index it.

- [ ] **Step 3.2: Replace `IDice` and `Dice`**

Replace `src/Questline/Engine/Characters/IDice.cs` with:

```csharp
namespace Questline.Engine.Characters;

public interface IDice
{
    int Roll(int sides);
    int Roll(int count, int sides);
}

public class Dice : IDice
{
    public int Roll(int sides) =>
        Random.Shared.Next(1, sides + 1);

    public int Roll(int count, int sides)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be at least 1.");
        }

        var total = 0;
        for (var i = 0; i < count; i++)
        {
            total += Roll(sides);
        }
        return total;
    }
}
```

`Roll(count, sides)` returns the **sum** of the dice (3d6 → 3..18).

- [ ] **Step 3.3: Replace `FakeDice` to support scalar surface**

Replace `tests/Questline.Tests/TestHelpers/FakeDice.cs` with:

```csharp
using Questline.Engine.Characters;

namespace Questline.Tests.TestHelpers;

public class FakeDice(params int[] results) : IDice
{
    private int _index;

    public int Roll(int sides) =>
        results[_index++];

    public int Roll(int count, int sides)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        var total = 0;
        for (var i = 0; i < count; i++)
        {
            total += results[_index++];
        }
        return total;
    }
}
```

`FakeDice` returns scripted values from the `results` array, advancing one slot per individual die rolled. So `new FakeDice(3, 5, 6).Roll(3, 6)` returns `3 + 5 + 6 = 14`.

- [ ] **Step 3.4: Update `AbilityScoresCalculator`**

Open `src/Questline/Domain/Characters/AbilityScoresCalculator.cs`. The current method probably calls `dice.Roll(3, 6)` (or `dice.Roll(3, 6).Sum()`) six times. Update each call site:

- Old: `dice.Roll(3, 6).Sum()` → New: `dice.Roll(3, 6)` (the new method returns the sum).
- Old: `dice.Roll(3, 6)[0] + dice.Roll(3, 6)[1] + ...` → New: `dice.Roll(3, 6)`.

Specifically:

```csharp
public static class AbilityScoresCalculator
{
    public static AbilityScores Calculate(IDice dice) =>
        new(
            new AbilityScore(dice.Roll(3, 6)),
            new AbilityScore(dice.Roll(3, 6)),
            new AbilityScore(dice.Roll(3, 6)),
            new AbilityScore(dice.Roll(3, 6)),
            new AbilityScore(dice.Roll(3, 6)),
            new AbilityScore(dice.Roll(3, 6)));
}
```

(If the actual current code is structured differently, preserve its structure — just swap the array call for the scalar one.)

- [ ] **Step 3.5: Update `HitPointsCalculator` (or equivalent)**

Locate the HP roll. Likely:

```csharp
public static class HitPointsCalculator
{
    public static HitPoints Calculate(CharacterClass? characterClass, IDice dice)
    {
        var hp = dice.Roll(1, 8);   // 1d8 for Fighter
        return new HitPoints(max: hp, current: hp);
    }
}
```

Update so:
- The call to `dice.Roll(...)` returns `int`, not `int[]`.
- The `new HitPoints(...)` call uses the new constructor (`max:`, `current:`).

- [ ] **Step 3.6: Update existing tests that construct `FakeDice` for character creation**

Find all `FakeDice(...)` usages:

```bash
git grep -n "new FakeDice" tests/
```

Each existing usage assumes the array-returning `Roll`. Now `FakeDice` advances the index per die. The numbers passed to `FakeDice(...)` should already be the individual dice values, so most tests likely still work — but verify each one. If a test passed `new FakeDice(15)` expecting `Roll(3,6)` to return `[15]`, it will now fail (it will index out of range). Test authors should pass three values, e.g. `new FakeDice(5, 5, 5)` for a 3d6 sum of 15.

A typical existing test might look like `new FakeDice(15, 15, 15, 15, 15, 15, 8)` for "six ability scores plus 1d8 HP". Under the new model that becomes `new FakeDice(5, 5, 5, 5, 5, 5, ...)` — six 3d6 rolls = 18 individual values, plus 1d8 = 1 more value = 19 ints total.

Update affected character-creation tests accordingly.

- [ ] **Step 3.7: Run all tests; expect pass**

```bash
dotnet build && dotnet test --no-build
```

Expected: green. If FakeDice-using tests fail, fix the result arrays per the previous step.

- [ ] **Step 3.8: Commit**

```bash
git add src/ tests/
git commit -m "Redesign IDice with scalar Roll overloads

Roll(sides) returns one die; Roll(count, sides) returns the sum of
count dice. FakeDice scripts individual die values per roll. Update
AbilityScoresCalculator, HitPointsCalculator, and affected tests.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

### Task 4: Add `DiceExtensions.Check` and `CheckResult`

A standard d20 check primitive: roll 1d20, add modifier, compare to DC.

**Files:**

- Create: `src/Questline/Engine/Characters/DiceExtensions.cs`
- Create: `tests/Questline.Tests/Engine/Characters/DiceExtensions/When_check_succeeds.cs`
- Create: `tests/Questline.Tests/Engine/Characters/DiceExtensions/When_check_fails.cs`

- [ ] **Step 4.1: Write the failing tests**

Create `tests/Questline.Tests/Engine/Characters/DiceExtensions/When_check_succeeds.cs`:

```csharp
using Questline.Engine.Characters;
using Questline.Tests.TestHelpers;
using Shouldly;

namespace Questline.Tests.Engine.Characters.DiceExtensions;

public class When_check_succeeds
{
    [Fact]
    public void Returns_success_when_roll_plus_modifier_meets_dc()
    {
        var dice = new FakeDice(11);    // d20

        var result = dice.Check(modifier: 1, dc: 12);

        result.Roll.ShouldBe(11);
        result.Modifier.ShouldBe(1);
        result.DC.ShouldBe(12);
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public void Returns_success_when_roll_plus_modifier_exceeds_dc()
    {
        var dice = new FakeDice(20);

        var result = dice.Check(modifier: 0, dc: 10);

        result.Success.ShouldBeTrue();
    }
}
```

Create `tests/Questline.Tests/Engine/Characters/DiceExtensions/When_check_fails.cs`:

```csharp
using Questline.Engine.Characters;
using Questline.Tests.TestHelpers;
using Shouldly;

namespace Questline.Tests.Engine.Characters.DiceExtensions;

public class When_check_fails
{
    [Fact]
    public void Returns_failure_when_roll_plus_modifier_below_dc()
    {
        var dice = new FakeDice(5);

        var result = dice.Check(modifier: 2, dc: 12);

        result.Roll.ShouldBe(5);
        result.Modifier.ShouldBe(2);
        result.DC.ShouldBe(12);
        result.Success.ShouldBeFalse();
    }
}
```

- [ ] **Step 4.2: Run tests; expect failure**

```bash
dotnet test --no-build --filter "FullyQualifiedName~DiceExtensions"
```

Expected: compile error — `Check` extension and `CheckResult` not defined.

- [ ] **Step 4.3: Implement `DiceExtensions` and `CheckResult`**

Create `src/Questline/Engine/Characters/DiceExtensions.cs`:

```csharp
namespace Questline.Engine.Characters;

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

- [ ] **Step 4.4: Run tests; expect pass**

```bash
dotnet test --no-build --filter "FullyQualifiedName~DiceExtensions"
```

Expected: PASS.

- [ ] **Step 4.5: Commit**

```bash
git add src/ tests/
git commit -m "Add DiceExtensions.Check and CheckResult

d20 check primitive: roll + modifier vs DC. Returns CheckResult with
the raw roll, modifier, DC, and success flag. Used by Phase 2+ skill
checks; not called by production code in Phase 1.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

---

## Tier 2 — Actor types

### Task 5: Define `Actor` types and `RequiresActorAttribute`

**Files:**

- Create: `src/Questline/Framework/Mediator/Actor.cs`
- Create: `src/Questline/Framework/Mediator/RequiresActorAttribute.cs`

- [ ] **Step 5.1: Create the `Actor` discriminated union**

Create `src/Questline/Framework/Mediator/Actor.cs`:

```csharp
using Questline.Domain.Characters.Entity;

namespace Questline.Framework.Mediator;

public abstract record Actor;

public sealed record NoActor : Actor;

public sealed record PartyActor : Actor;

public sealed record CharacterActor(Character Character) : Actor;
```

- [ ] **Step 5.2: Create the `RequiresActorAttribute`**

Create `src/Questline/Framework/Mediator/RequiresActorAttribute.cs`:

```csharp
namespace Questline.Framework.Mediator;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class RequiresActorAttribute : Attribute;
```

- [ ] **Step 5.3: Verify build**

```bash
dotnet build
```

Expected: green (no usages yet, types just exist).

- [ ] **Step 5.4: Commit**

```bash
git add src/
git commit -m "Add Actor discriminated union and RequiresActorAttribute

Actor variants: NoActor (system commands like login/version),
PartyActor (party-level command in Playing phase), and CharacterActor
(scoped to a specific PC). RequiresActorAttribute marks requests with
no party-level meaning; honoured by the Parser in Task 11.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

---

## Tier 3 — Handler signature refactor

This tier changes the handler interface signature. All handlers, the `RequestSender`, and the calling sites in `GameEngine` must update together. No behavioural change yet — handlers ignore the new `Actor` parameter for now.

### Task 6: Add `Actor` parameter to handler pipeline

**Files:**

- Modify: `src/Questline/Framework/Mediator/IRequestHandler.cs`
- Modify: `src/Questline/Framework/Mediator/RequestSender.cs`
- Modify: every file under `src/Questline/Engine/Handlers/` (10 files)
- Modify: `src/Questline/Engine/Core/GameEngine.cs` (call sites of `dispatcher.Send`)
- Modify: every test in `tests/Questline.Tests/Engine/Handlers/` (each handler test invokes the handler)
- Modify: `tests/Questline.Tests/Framework/Mediator/RequestSender/When_sending_request.cs`

- [ ] **Step 6.1: Update `IRequestHandler<T>` signature**

Replace `src/Questline/Framework/Mediator/IRequestHandler.cs` with:

```csharp
namespace Questline.Framework.Mediator;

public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
{
    Task<IResponse> Handle(Actor actor, TRequest request);
}
```

- [ ] **Step 6.2: Update `RequestSender` to forward `Actor`**

Replace `src/Questline/Framework/Mediator/RequestSender.cs` with:

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace Questline.Framework.Mediator;

public class RequestSender(IServiceProvider serviceProvider)
{
    public async Task<IResponse> Send(Actor actor, IRequest request)
    {
        var requestType = request.GetType();

        var requestHandlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        var handleMethod       = requestHandlerType.GetMethod(nameof(IRequestHandler<>.Handle))!;

        var handler = serviceProvider.GetRequiredService(requestHandlerType);

        var task = (Task<IResponse>)handleMethod.Invoke(handler, [actor, request])!;
        return await task;
    }
}
```

- [ ] **Step 6.3: Update all 10 handlers**

Each handler's `Handle` method gains `Actor actor` as the first parameter. Behaviour is unchanged in this task. For each handler in `src/Questline/Engine/Handlers/*.cs`:

```csharp
// Before
public Task<IResponse> Handle(Requests.LoginCommand request) =>
    Task.FromResult<IResponse>(...);

// After
public Task<IResponse> Handle(Actor actor, Requests.LoginCommand request) =>
    Task.FromResult<IResponse>(...);
```

For async handlers:

```csharp
// Before
public async Task<IResponse> Handle(Requests.TakeItemCommand request)
{
    var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);
    // ...
}

// After
public async Task<IResponse> Handle(Actor actor, Requests.TakeItemCommand request)
{
    var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);
    // ...
}
```

Add `using Questline.Framework.Mediator;` if not already present.

The 10 handlers to update:

- `LoginCommandHandler`
- `GetRoomDetailsHandler`
- `MovePlayerCommandHandler`
- `TakeItemHandler`
- `DropItemCommandHandler`
- `GetPlayerInventoryQueryHandler`
- `QuitGameHandler`
- `UseItemCommandHandler`
- `ExamineCommandHandler`
- `VersionQueryHandler`

- [ ] **Step 6.4: Update `GameEngine` call sites**

In `src/Questline/Engine/Core/GameEngine.cs`:

Replace:
```csharp
var response = await dispatcher.Send(parseResult.Request!);
```

In `HandleLogin` (login is a system command, no party context):
```csharp
var response = await dispatcher.Send(new NoActor(), parseResult.Request!);
```

In `HandleGamePlay` (party-level placeholder until Task 11 introduces parser-driven actor detection):
```csharp
var response = await dispatcher.Send(new PartyActor(), parseResult.Request!);
```

Add `using Questline.Framework.Mediator;` if needed.

- [ ] **Step 6.5: Update all handler tests**

Every handler test invokes `handler.Handle(...)` directly. Each call must now pass an `Actor`. For login and version tests pass `new NoActor()`; for everything else pass `new PartyActor()`.

Example for `TakeItemHandler/When_item_is_in_room.cs`:

```csharp
// Before
var response = await handler.Handle(new TakeItemCommand("rusty key"));

// After
var response = await handler.Handle(new PartyActor(), new TakeItemCommand("rusty key"));
```

Repeat across all handler test files. Use `git grep -n "handler.Handle(" tests/` to find every site.

For `tests/Questline.Tests/Framework/Mediator/RequestSender/When_sending_request.cs`, update the `Send` invocation:

```csharp
// Before
await sender.Send(request);

// After
await sender.Send(new NoActor(), request);
```

And update the fake handler in that test file to match the new interface signature.

- [ ] **Step 6.6: Run all tests; expect pass**

```bash
dotnet build && dotnet test --no-build
```

Expected: all tests green. If any fail with type errors, scan for missed `Handle(` or `Send(` call sites.

- [ ] **Step 6.7: Commit**

```bash
git add src/ tests/
git commit -m "Add Actor parameter to handler pipeline

IRequestHandler<T>.Handle and RequestSender.Send gain a leading Actor
parameter. All 10 handlers and their tests are updated. GameEngine
passes NoActor for login and PartyActor (placeholder) for gameplay
commands; parser-driven actor selection lands in Task 11.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

---

## Tier 4 — Party entity & Character extension

### Task 7: Define the `Party` entity

**Files:**

- Create: `src/Questline/Domain/Parties/Entity/Party.cs`
- Create: `tests/Questline.Tests/Domain/Parties/Entity/Party/When_finding_member_by_name.cs`
- Create: `tests/Questline.Tests/Domain/Parties/Entity/Party/When_party_has_no_living_members.cs`

- [ ] **Step 7.1: Write the failing tests**

Create `tests/Questline.Tests/Domain/Parties/Entity/Party/When_finding_member_by_name.cs`:

```csharp
using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;

namespace Questline.Tests.Domain.Parties.Entity.Party;

public class When_finding_member_by_name
{
    [Fact]
    public void Returns_member_with_matching_name_case_insensitively()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var mira  = CharacterBuilder.New().WithName("Mira").Build();
        var party = new Questline.Domain.Parties.Entity.Party(id: "party-1", members: [aric, mira]);

        party.FindByName("aric").ShouldBe(aric);
        party.FindByName("MIRA").ShouldBe(mira);
    }

    [Fact]
    public void Returns_null_when_no_member_matches()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var party = new Questline.Domain.Parties.Entity.Party(id: "party-1", members: [aric]);

        party.FindByName("borin").ShouldBeNull();
    }
}
```

Create `tests/Questline.Tests/Domain/Parties/Entity/Party/When_party_has_no_living_members.cs`:

```csharp
using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;

namespace Questline.Tests.Domain.Parties.Entity.Party;

public class When_party_has_no_living_members
{
    [Fact]
    public void Returns_empty_alive_collection()
    {
        var dead   = CharacterBuilder.New().WithName("Aric").WithHitPoints(max: 4, current: 0).Build();
        var party  = new Questline.Domain.Parties.Entity.Party(id: "party-1", members: [dead]);

        party.MembersAlive.ShouldBeEmpty();
    }

    [Fact]
    public void Returns_only_living_members()
    {
        var alive = CharacterBuilder.New().WithName("Mira").WithHitPoints(max: 4, current: 4).Build();
        var dead  = CharacterBuilder.New().WithName("Aric").WithHitPoints(max: 4, current: 0).Build();
        var party = new Questline.Domain.Parties.Entity.Party(id: "party-1", members: [dead, alive]);

        party.MembersAlive.ShouldBe([alive]);
    }
}
```

These tests use a `CharacterBuilder` test helper that does not yet exist. Create it in **Step 7.4**.

- [ ] **Step 7.2: Run tests; expect failure**

```bash
dotnet test --no-build --filter "FullyQualifiedName~Domain.Parties"
```

Expected: compile error — `Party` and `CharacterBuilder` not defined.

- [ ] **Step 7.3: Implement `Party`**

Create `src/Questline/Domain/Parties/Entity/Party.cs`:

```csharp
using Questline.Domain.Characters.Entity;
using Questline.Framework.Domain;

namespace Questline.Domain.Parties.Entity;

public class Party : DomainEntity
{
    public Party(string id, IReadOnlyList<Character> members)
    {
        Id      = id;
        Members = members;
    }

    public IReadOnlyList<Character> Members { get; }

    public IReadOnlyList<Character> MembersAlive =>
        Members.Where(c => c.HitPoints.IsAlive).ToList();

    public Character? FindByName(string name) =>
        Members.FirstOrDefault(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public bool IsMember(Character character) =>
        Members.Any(c => c.Id == character.Id);
}
```

- [ ] **Step 7.4: Add `CharacterBuilder` test helper**

Create `tests/Questline.Tests/TestHelpers/Builders/CharacterBuilder.cs`:

```csharp
using Questline.Domain.Characters.Entity;

namespace Questline.Tests.TestHelpers.Builders;

public class CharacterBuilder
{
    private string         _id            = Guid.NewGuid().ToString();
    private string         _name          = "Aric";
    private Race           _race          = Race.Human;
    private CharacterClass? _class        = null;
    private string         _occupation    = "Beekeeper";
    private AbilityScores  _abilityScores = new(
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10),
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10));
    private HitPoints      _hitPoints     = new(max: 4, current: 4);

    public static CharacterBuilder New() => new();

    public CharacterBuilder WithId(string id)            { _id = id; return this; }
    public CharacterBuilder WithName(string name)        { _name = name; return this; }
    public CharacterBuilder WithRace(Race race)          { _race = race; return this; }
    public CharacterBuilder WithOccupation(string s)     { _occupation = s; return this; }
    public CharacterBuilder WithAbilityScores(AbilityScores scores) { _abilityScores = scores; return this; }
    public CharacterBuilder WithHitPoints(int max, int current) { _hitPoints = new HitPoints(max, current); return this; }

    public Character Build() =>
        Character.Create(_id, _name, _race, _class, _hitPoints, _abilityScores, _occupation);
}
```

Note: `Character.Create` does not yet take `occupation`. Task 8 adds it. The `_occupation` parameter on the builder is wired up here so Task 8 can use it without changing the builder again. **Until Task 8, the builder's `Build()` call will fail to compile** — so **Task 7's tests will fail to compile until Task 8**. To keep the tasks independent and tests green at each step, temporarily build without occupation in Task 7:

Replace the `Build()` line above with:

```csharp
public Character Build() =>
    Character.Create(_id, _name, _race, _class, _hitPoints, _abilityScores);
```

Task 8 then updates `Build()` to forward `_occupation`.

- [ ] **Step 7.5: Run tests; expect pass**

```bash
dotnet test --no-build --filter "FullyQualifiedName~Domain.Parties"
```

Expected: PASS.

- [ ] **Step 7.6: Commit**

```bash
git add src/ tests/
git commit -m "Add Party domain entity and CharacterBuilder test helper

Party holds an ordered list of Characters (index 0 is the marching
order leader). Provides FindByName (case-insensitive), IsMember, and
MembersAlive.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

### Task 8: Extend `Character` with `Occupation`, nullable `Class`, and per-PC `Inventory`

**Files:**

- Modify: `src/Questline/Domain/Characters/Entity/Character.cs`
- Modify: `tests/Questline.Tests/TestHelpers/Builders/CharacterBuilder.cs` (forward occupation)
- Create: `tests/Questline.Tests/Domain/Characters/Entity/Character/When_managing_inventory.cs`

- [ ] **Step 8.1: Write a test for per-character inventory**

Create `tests/Questline.Tests/Domain/Characters/Entity/Character/When_managing_inventory.cs`:

```csharp
using Questline.Domain.Characters.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;

namespace Questline.Tests.Domain.Characters.Entity.Character;

public class When_managing_inventory
{
    [Fact]
    public void Add_item_appears_in_inventory()
    {
        var aric = CharacterBuilder.New().Build();
        var key  = new Item { Id = "rusty-key", Name = "rusty key", Description = "" };

        aric.AddInventoryItem(key);

        aric.Inventory.ShouldContain(key);
    }

    [Fact]
    public void Remove_item_takes_it_out_of_inventory()
    {
        var aric = CharacterBuilder.New().Build();
        var key  = new Item { Id = "rusty-key", Name = "rusty key", Description = "" };
        aric.AddInventoryItem(key);

        aric.RemoveInventoryItem(key);

        aric.Inventory.ShouldNotContain(key);
    }

    [Fact]
    public void Find_by_name_is_case_insensitive()
    {
        var aric = CharacterBuilder.New().Build();
        var key  = new Item { Id = "rusty-key", Name = "rusty key", Description = "" };
        aric.AddInventoryItem(key);

        aric.FindInventoryItemByName("RUSTY KEY").ShouldBe(key);
    }
}
```

- [ ] **Step 8.2: Run test; expect failure**

```bash
dotnet test --no-build --filter "FullyQualifiedName~Domain.Characters.Entity.Character.When_managing_inventory"
```

Expected: compile error — `AddInventoryItem` etc. not on `Character`.

- [ ] **Step 8.3: Update `Character` entity**

Replace `src/Questline/Domain/Characters/Entity/Character.cs` with:

```csharp
using Questline.Domain.Characters.Data;
using Questline.Domain.Shared.Entity;
using Questline.Framework.Domain;

namespace Questline.Domain.Characters.Entity;

public class Character : DomainEntity
{
    private readonly List<Item> _inventory = [];

    public string          Name          { get; private init; } = null!;
    public Race            Race          { get; private init; }
    public CharacterClass? Class         { get; private init; }
    public int             Level         { get; private init; }
    public int             Experience    { get; init; }
    public string          Occupation    { get; private init; } = "";
    public AbilityScores   AbilityScores { get; private init; } = null!;
    public HitPoints       HitPoints     { get; private init; } = null!;

    public IReadOnlyList<Item> Inventory
    {
        get => _inventory;
        init => _inventory = [..value];
    }

    public static Character Create(
        string          id,
        string          name,
        Race            race,
        CharacterClass? characterClass,
        HitPoints       hitPoints,
        AbilityScores   abilityScores,
        string          occupation = "",
        int             level = 0)
    {
        return new Character
        {
            Id            = id,
            Name          = name,
            Race          = race,
            Class         = characterClass,
            Level         = level,
            Experience    = 0,
            Occupation    = occupation,
            AbilityScores = abilityScores,
            HitPoints     = hitPoints
        };
    }

    public void AddInventoryItem(Item item) => _inventory.Add(item);

    public void RemoveInventoryItem(Item item) => _inventory.Remove(item);

    public Item? FindInventoryItemByName(string name) =>
        _inventory.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public CharacterSummary ToSummary() =>
        new(
            Name,
            Race.ToString(),
            Class?.ToString() ?? "Level 0",
            Level,
            Experience,
            HitPoints.Max,
            HitPoints.Current,
            new AbilityScoresSummary(
                AbilityScores.Strength.Score,
                AbilityScores.Intelligence.Score,
                AbilityScores.Wisdom.Score,
                AbilityScores.Dexterity.Score,
                AbilityScores.Constitution.Score,
                AbilityScores.Charisma.Score));
}
```

Key changes:
- `Class` is nullable.
- `Level` defaults to `0` (level-0 funnel start).
- `Occupation` field added.
- Per-character `Inventory` list with `AddInventoryItem`/`RemoveInventoryItem`/`FindInventoryItemByName`.
- `Race` parameter is no longer nullable in `Create` (callers always have it).
- `ToSummary()` shows `"Level 0"` when class is null.

- [ ] **Step 8.4: Update `CharacterBuilder` to pass occupation**

In `tests/Questline.Tests/TestHelpers/Builders/CharacterBuilder.cs`, update the `Build()` method to:

```csharp
public Character Build() =>
    Character.Create(_id, _name, _race, _class, _hitPoints, _abilityScores, _occupation);
```

- [ ] **Step 8.5: Update `CharacterCreationStateMachine` to pass through new fields**

In `src/Questline/Engine/Characters/CharacterCreationStateMachine.cs` `ProcessCharacterName`, the call to `Character.Create` must change because `Race` is no longer nullable in `Create` and `occupation` is new.

Update the call (around line 136):

```csharp
_completedCharacter = Character.Create(
    Guid.NewGuid().ToString(),
    _context.Name,
    _context.Race ?? Race.Human,
    _context.Class,
    _context.HitPoints,
    _context.AbilityScores,
    occupation: "");
```

This module is removed in Task 18; for now the call just compiles. The character it produces still works as a level-1 character in the existing flow.

Also note: `Character.Create` now defaults `level: 0`. Pre-existing single-character creation should produce level-1 characters. Pass `level: 1` explicitly:

```csharp
_completedCharacter = Character.Create(
    Guid.NewGuid().ToString(),
    _context.Name,
    _context.Race ?? Race.Human,
    _context.Class,
    _context.HitPoints,
    _context.AbilityScores,
    occupation: "",
    level: 1);
```

- [ ] **Step 8.6: Update existing tests that hit `Character.Create`**

Any test that calls `Character.Create(..., race: null, ...)` won't compile (race is no longer nullable). Find them:

```bash
git grep -n "Character.Create" src/ tests/
```

Update test usages to pass a non-null `Race`. The race choice is irrelevant for most tests; pass `Race.Human` as default.

- [ ] **Step 8.7: Run all tests; expect pass**

```bash
dotnet build && dotnet test --no-build
```

Expected: green.

- [ ] **Step 8.8: Commit**

```bash
git add src/ tests/
git commit -m "Extend Character: nullable Class, Occupation, per-PC Inventory

Class is nullable (null at level 0). Occupation is a flavour string.
Inventory moves onto Character with Add/Remove/FindByName. Default
Level changes to 0; the existing single-character creation flow now
explicitly passes level: 1.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

---

## Tier 5 — Playthrough refactor

This is the largest task. `Playthrough` collapses its denormalised character fields and exposes a `Party` instead. The persistence document and mapper change shape. To keep the rest of the codebase compiling and tests green, the existing inventory/character methods on `Playthrough` are preserved as **delegates to the leader (`Party.Members[0]`)**.

### Task 9: Add `Turns` and `Party` to `Playthrough`; collapse denormalised fields

**Files:**

- Modify: `src/Questline/Domain/Playthroughs/Entity/Playthrough.cs`
- Modify: `src/Questline/Engine/Persistence/Playthroughs/PlaythroughDocument.cs`
- Modify: `src/Questline/Engine/Persistence/Playthroughs/PlaythroughMapper.cs`
- Modify: `src/Questline/Engine/Core/GameEngine.cs` (`Playthrough.Create` callers, `ToCharacterSummary` callers)
- Modify: `tests/Questline.Tests/TestHelpers/Builders/PlaythroughBuilder.cs`
- Modify: tests in `tests/Questline.Tests/Domain/Playthroughs/Entity/Playthrough/`

- [ ] **Step 9.1: Refactor `Playthrough` to host a `Party`**

Replace `src/Questline/Domain/Playthroughs/Entity/Playthrough.cs` with:

```csharp
using Questline.Domain.Characters.Data;
using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Framework.Domain;

namespace Questline.Domain.Playthroughs.Entity;

public class Playthrough : DomainEntity
{
    private readonly HashSet<string>                _unlockedBarriers = [];
    private readonly Dictionary<string, List<Item>> _roomItems        = new();

    public required string   Username       { get; init; }
    public required string   AdventureId    { get; init; }
    public required string   StartingRoomId { get; init; }
    public required Party    Party          { get; init; }
    public required string   Location       { get; set; }
    public          int      Turns          { get; private set; }

    public IReadOnlyCollection<string> UnlockedBarriers
    {
        get => _unlockedBarriers;
        init => _unlockedBarriers = [..value];
    }

    public IReadOnlyDictionary<string, List<Item>> RoomItems
    {
        get => _roomItems;
        init => _roomItems = new Dictionary<string, List<Item>>(value);
    }

    /// <summary>Leader-shorthand: returns the leader's inventory.</summary>
    public IReadOnlyList<Item> Inventory => Party.Members[0].Inventory;

    public static Playthrough Create(
        string username,
        string adventureId,
        string startingRoomId,
        Party  party)
    {
        return new Playthrough
        {
            Id             = Guid.NewGuid().ToString(),
            Username       = username,
            AdventureId    = adventureId,
            StartingRoomId = startingRoomId,
            Party          = party,
            Location       = startingRoomId
        };
    }

    public void MoveTo(string locationId) => Location = locationId;

    public void IncrementTurns() => Turns++;

    /// <summary>Leader-shorthand for the current single-character APIs. Phase 1 handlers
    /// override this with per-actor routing in Tier 7.</summary>
    public void AddInventoryItem(Item item) => Party.Members[0].AddInventoryItem(item);

    public void RemoveInventoryItem(Item item) => Party.Members[0].RemoveInventoryItem(item);

    public Item? FindInventoryItemByName(string name) =>
        Party.Members[0].FindInventoryItemByName(name);

    public bool IsBarrierUnlocked(string barrierId) => _unlockedBarriers.Contains(barrierId);

    public void UnlockBarrier(string barrierId) => _unlockedBarriers.Add(barrierId);

    public List<Item>? GetRecordedRoomItems(string roomId) =>
        _roomItems.TryGetValue(roomId, out var items) ? items : null;

    public void RecordRoomItems(string roomId, List<Item> items) =>
        _roomItems[roomId] = items;

    public PartySummary ToPartySummary() =>
        new(Party.Members.Select(c => c.ToSummary()).ToList(), Turns);
}
```

Note: this introduces a new `PartySummary` type. Define it next.

- [ ] **Step 9.2: Add `PartySummary` data type**

Create `src/Questline/Domain/Playthroughs/Data/PartySummary.cs`:

```csharp
using Questline.Domain.Characters.Data;

namespace Questline.Domain.Playthroughs.Data;

public record PartySummary(IReadOnlyList<CharacterSummary> Members, int Turns);
```

- [ ] **Step 9.3: Update `PlaythroughDocument`**

Replace `src/Questline/Engine/Persistence/Playthroughs/PlaythroughDocument.cs` with:

```csharp
using Questline.Engine.Persistence.Rooms;
using Questline.Framework.Persistence;

namespace Questline.Engine.Persistence.Playthroughs;

public class PlaythroughDocument : Document
{
    public string                                  Username         { get; set; } = null!;
    public string                                  AdventureId      { get; set; } = null!;
    public string                                  StartingRoomId   { get; set; } = null!;
    public string                                  Location         { get; set; } = null!;
    public int                                     Turns            { get; set; }
    public PartyDocument                           Party            { get; set; } = null!;
    public List<string>                            UnlockedBarriers { get; set; } = [];
    public Dictionary<string, List<ItemDocument>>  RoomItems        { get; set; } = new();
}

public class PartyDocument
{
    public List<CharacterDocument> Members { get; set; } = [];
}

public class CharacterDocument
{
    public string                Id            { get; set; } = null!;
    public string                Name          { get; set; } = null!;
    public string                Race          { get; set; } = null!;
    public string?               Class         { get; set; }
    public int                   Level         { get; set; }
    public int                   Experience    { get; set; }
    public string                Occupation    { get; set; } = "";
    public AbilityScoresDocument AbilityScores { get; set; } = null!;
    public HitPointsDocument     HitPoints     { get; set; } = null!;
    public List<ItemDocument>    Inventory     { get; set; } = [];
}

public class AbilityScoresDocument
{
    public int Strength     { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom       { get; set; }
    public int Dexterity    { get; set; }
    public int Constitution { get; set; }
    public int Charisma     { get; set; }
}

public class HitPointsDocument
{
    public int MaxHitPoints     { get; set; }
    public int CurrentHitPoints { get; set; }
}
```

- [ ] **Step 9.4: Update `PlaythroughMapper`**

Replace `src/Questline/Engine/Persistence/Playthroughs/PlaythroughMapper.cs` with:

```csharp
using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Domain.Playthroughs.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Persistence.Rooms;
using Questline.Framework.Persistence;

namespace Questline.Engine.Persistence.Playthroughs;

public class PlaythroughMapper : IPersistenceMapper<Playthrough, PlaythroughDocument>
{
    public Playthrough From(PlaythroughDocument document) => new()
    {
        Id               = document.Id,
        Username         = document.Username,
        AdventureId      = document.AdventureId,
        StartingRoomId   = document.StartingRoomId,
        Location         = document.Location,
        Party            = MapParty(document.Party),
        UnlockedBarriers = document.UnlockedBarriers.ToHashSet(),
        RoomItems        = document.RoomItems.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Select(MapItem).ToList())
    };

    public PlaythroughDocument To(Playthrough entity) => new()
    {
        Id               = entity.Id,
        Username         = entity.Username,
        AdventureId      = entity.AdventureId,
        StartingRoomId   = entity.StartingRoomId,
        Location         = entity.Location,
        Turns            = entity.Turns,
        Party            = new PartyDocument
        {
            Members = entity.Party.Members.Select(MapCharacterToDoc).ToList()
        },
        UnlockedBarriers = entity.UnlockedBarriers.ToList(),
        RoomItems        = entity.RoomItems.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Select(MapItemToDoc).ToList())
    };

    private static Party MapParty(PartyDocument doc) =>
        new(
            id: Guid.NewGuid().ToString(),
            members: doc.Members.Select(MapCharacterFromDoc).ToList());

    private static Character MapCharacterFromDoc(CharacterDocument doc)
    {
        var character = Character.Create(
            id: doc.Id,
            name: doc.Name,
            race: Enum.Parse<Race>(doc.Race),
            characterClass: doc.Class is null ? null : Enum.Parse<CharacterClass>(doc.Class),
            hitPoints: new HitPoints(max: doc.HitPoints.MaxHitPoints, current: doc.HitPoints.CurrentHitPoints),
            abilityScores: new AbilityScores(
                new AbilityScore(doc.AbilityScores.Strength),
                new AbilityScore(doc.AbilityScores.Intelligence),
                new AbilityScore(doc.AbilityScores.Wisdom),
                new AbilityScore(doc.AbilityScores.Dexterity),
                new AbilityScore(doc.AbilityScores.Constitution),
                new AbilityScore(doc.AbilityScores.Charisma)),
            occupation: doc.Occupation,
            level: doc.Level);

        foreach (var item in doc.Inventory)
        {
            character.AddInventoryItem(MapItem(item));
        }

        return character;
    }

    private static CharacterDocument MapCharacterToDoc(Character character) => new()
    {
        Id         = character.Id,
        Name       = character.Name,
        Race       = character.Race.ToString(),
        Class      = character.Class?.ToString(),
        Level      = character.Level,
        Experience = character.Experience,
        Occupation = character.Occupation,
        AbilityScores = new AbilityScoresDocument
        {
            Strength     = character.AbilityScores.Strength.Score,
            Intelligence = character.AbilityScores.Intelligence.Score,
            Wisdom       = character.AbilityScores.Wisdom.Score,
            Dexterity    = character.AbilityScores.Dexterity.Score,
            Constitution = character.AbilityScores.Constitution.Score,
            Charisma     = character.AbilityScores.Charisma.Score
        },
        HitPoints = new HitPointsDocument
        {
            MaxHitPoints     = character.HitPoints.Max,
            CurrentHitPoints = character.HitPoints.Current
        },
        Inventory = character.Inventory.Select(MapItemToDoc).ToList()
    };

    private static Item MapItem(ItemDocument doc) => new()
    {
        Id          = doc.Id,
        Name        = doc.Name,
        Description = doc.Description
    };

    private static ItemDocument MapItemToDoc(Item item) => new()
    {
        Id          = item.Id,
        Name        = item.Name,
        Description = item.Description
    };
}
```

Note the `From` method does not set `Turns` because `Turns` has a `private set` and isn't in the init list. We'll need to expose a way to restore it. Update the `Playthrough` class: change `public int Turns { get; private set; }` to use a non-init backing path. Simplest: add a private setter with `init`:

```csharp
public int Turns { get; private set; }
```

stays as is. Add a method or factory path to restore turns. Easiest: add an internal initializer:

In `Playthrough.cs`, replace the `Turns` declaration with:

```csharp
public int Turns { get; private set; }

internal void RestoreTurns(int turns) => Turns = turns;
```

Then in the mapper's `From` method, after constructing the entity, call `playthrough.RestoreTurns(document.Turns)`:

```csharp
public Playthrough From(PlaythroughDocument document)
{
    var playthrough = new Playthrough
    {
        Id               = document.Id,
        Username         = document.Username,
        AdventureId      = document.AdventureId,
        StartingRoomId   = document.StartingRoomId,
        Location         = document.Location,
        Party            = MapParty(document.Party),
        UnlockedBarriers = document.UnlockedBarriers.ToHashSet(),
        RoomItems        = document.RoomItems.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Select(MapItem).ToList())
    };

    playthrough.RestoreTurns(document.Turns);
    return playthrough;
}
```

This keeps `Turns` mutation contained to gameplay (`IncrementTurns`) and persistence (`RestoreTurns`).

- [ ] **Step 9.5: Update `GameEngine` callers**

In `src/Questline/Engine/Core/GameEngine.cs`:

The `HandleCharacterCreation` method calls `Playthrough.Create(..., character)`. The new `Playthrough.Create` takes a `Party`. The character-creation flow currently produces a single `Character`. Wrap it in a one-member `Party` for now (Task 18 replaces this with proper `PartyCreation`):

Replace:

```csharp
var playthrough = Playthrough.Create(gameSession.Username!, _selectedAdventureId, adventure.StartingRoomId, character);
```

With:

```csharp
var party       = new Party(id: Guid.NewGuid().ToString(), members: [character]);
var playthrough = Playthrough.Create(gameSession.Username!, _selectedAdventureId, adventure.StartingRoomId, party);
```

Add `using Questline.Domain.Parties.Entity;` if not already present.

Also: the `StartAdventure` method calls `playthrough.ToCharacterSummary()` for `AdventureStartedResponse`. The new entity doesn't have that method — it has `ToPartySummary()`. The existing response shape takes a `CharacterSummary`. **For now** (until Tier 8 reworks the stats display), pass the leader's summary:

Replace:

```csharp
playthrough.ToCharacterSummary(),
```

With:

```csharp
playthrough.Party.Members[0].ToSummary(),
```

This keeps `AdventureStartedResponse` working with a single character summary.

- [ ] **Step 9.6: Update `PlaythroughBuilder`**

Read `tests/Questline.Tests/TestHelpers/Builders/PlaythroughBuilder.cs` to see the current shape. The builder constructs a `Playthrough`. It must now build a `Party` first.

Update the builder to expose a `WithParty` or `WithCharacter` method, defaulting to a single-member party. Sketch:

```csharp
using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Domain.Playthroughs.Entity;

namespace Questline.Tests.TestHelpers.Builders;

public class PlaythroughBuilder
{
    private string                _id             = Guid.NewGuid().ToString();
    private string                _username       = "tester";
    private string                _adventureId    = "test-adventure";
    private string                _startingRoomId = "start";
    private string                _location       = "start";
    private List<Character>       _members        = [];

    public static PlaythroughBuilder New() => new();

    public PlaythroughBuilder WithId(string id) { _id = id; return this; }
    public PlaythroughBuilder WithUsername(string u) { _username = u; return this; }
    public PlaythroughBuilder WithAdventure(string id) { _adventureId = id; return this; }
    public PlaythroughBuilder WithStartingRoom(string id) { _startingRoomId = id; _location = id; return this; }
    public PlaythroughBuilder WithLocation(string id) { _location = id; return this; }
    public PlaythroughBuilder WithCharacter(Character c) { _members.Add(c); return this; }

    public Playthrough Build()
    {
        var members = _members.Count > 0 ? _members : [CharacterBuilder.New().Build()];
        var party   = new Party(id: Guid.NewGuid().ToString(), members: members);

        return new Playthrough
        {
            Id             = _id,
            Username       = _username,
            AdventureId    = _adventureId,
            StartingRoomId = _startingRoomId,
            Location       = _location,
            Party          = party
        };
    }
}
```

Adjust to match the existing builder's API surface where possible (the existing one likely has more methods that take denormalised character fields — convert them to either no-ops for now or populate a single-character party).

- [ ] **Step 9.7: Update existing `Playthrough` tests**

Tests under `tests/Questline.Tests/Domain/Playthroughs/Entity/Playthrough/` use the old denormalised API in their assertions. With the leader-delegation methods preserved on `Playthrough`, most of these should still compile.

Specific files likely affected:

- `When_managing_inventory.cs` — still works (delegates to leader).
- `When_interacting_with_barriers.cs` — still works (no character changes).
- `When_moving.cs` — still works.
- `When_recording_room_items.cs` — still works.
- `When_converting_to_summary.cs` — **breaks** because `ToCharacterSummary` is replaced by `ToPartySummary`. Either rename the test file or assert against `ToPartySummary` and check the leader summary inside.

Update `When_converting_to_summary.cs` to test `ToPartySummary` returning a `PartySummary` whose `Members[0]` is the leader's `CharacterSummary` and whose `Turns` matches.

- [ ] **Step 9.8: Run all tests; expect pass**

```bash
dotnet build && dotnet test --no-build
```

If anything fails: most likely the `PlaythroughBuilder` API mismatch. Fix the builder to expose the methods callers actually use; the existing tests are the canonical surface.

- [ ] **Step 9.9: Commit**

```bash
git add src/ tests/
git commit -m "Refactor Playthrough to host a Party; add Turns counter

Playthrough denormalised character fields collapse into a Party
reference. Existing single-character APIs (Inventory, AddInventoryItem,
etc.) preserved as leader-shorthand for backwards compatibility with
handlers — Tier 7 redirects them to per-actor routing. PlaythroughDocument
gains PartyDocument with nested CharacterDocument; PlaythroughMapper
hydrates the party including each PC's inventory. Adds RestoreTurns
internal init for persistence.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

---

## Tier 6 — Parser actor-prefix detection

### Task 10: Detect actor prefix and propagate `Actor` through `ParseResult`

**Files:**

- Modify: `src/Questline/Engine/Parsers/ParseResult.cs`
- Modify: `src/Questline/Engine/Parsers/Parser.cs`
- Modify: `src/Questline/Engine/Core/GameEngine.cs` (forward Actor from ParseResult)
- Create: `tests/Questline.Tests/Engine/Parsers/Parser/When_input_starts_with_actor_name.cs`
- Create: `tests/Questline.Tests/Engine/Parsers/Parser/When_input_has_no_actor_prefix.cs`
- Modify: `tests/Questline.Tests/Engine/Parsers/Parser/When_parsing_valid_input.cs` (asserts unchanged behaviour with no party)

- [ ] **Step 10.1: Write tests for actor-prefix parsing**

Create `tests/Questline.Tests/Engine/Parsers/Parser/When_input_starts_with_actor_name.cs`:

```csharp
using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;
using static Questline.Engine.Messages.Requests;

namespace Questline.Tests.Engine.Parsers.Parser;

public class When_input_starts_with_actor_name
{
    private readonly Questline.Engine.Parsers.Parser _parser = new();

    [Fact]
    public void Routes_command_to_named_character()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var mira  = CharacterBuilder.New().WithName("Mira").Build();
        var party = new Party(id: "party-1", members: [aric, mira]);

        var result = _parser.Parse("aric look", party);

        result.IsSuccess.ShouldBeTrue();
        result.Actor.ShouldBeOfType<CharacterActor>();
        ((CharacterActor)result.Actor!).Character.ShouldBe(aric);
        result.Request.ShouldBeOfType<GetRoomDetailsQuery>();
    }

    [Fact]
    public void Matches_actor_name_case_insensitively()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var party = new Party(id: "party-1", members: [aric]);

        var result = _parser.Parse("ARIC look", party);

        result.IsSuccess.ShouldBeTrue();
        result.Actor.ShouldBeOfType<CharacterActor>();
    }
}
```

Create `tests/Questline.Tests/Engine/Parsers/Parser/When_input_has_no_actor_prefix.cs`:

```csharp
using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;
using static Questline.Engine.Messages.Requests;

namespace Questline.Tests.Engine.Parsers.Parser;

public class When_input_has_no_actor_prefix
{
    private readonly Questline.Engine.Parsers.Parser _parser = new();

    [Fact]
    public void Returns_party_actor()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var party = new Party(id: "party-1", members: [aric]);

        var result = _parser.Parse("look", party);

        result.IsSuccess.ShouldBeTrue();
        result.Actor.ShouldBeOfType<PartyActor>();
        result.Request.ShouldBeOfType<GetRoomDetailsQuery>();
    }

    [Fact]
    public void Treats_unmatched_first_token_as_verb_attempt()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var party = new Party(id: "party-1", members: [aric]);

        var result = _parser.Parse("borin look", party);

        // 'borin' isn't a PC and isn't a verb, so the parser fails.
        result.IsSuccess.ShouldBeFalse();
    }
}
```

- [ ] **Step 10.2: Run tests; expect failure**

```bash
dotnet test --no-build --filter "FullyQualifiedName~Parser.When_input"
```

Expected: compile error — `Parse(string, Party)` overload and `ParseResult.Actor` not defined.

- [ ] **Step 10.3: Extend `ParseResult` with `Actor`**

Replace `src/Questline/Engine/Parsers/ParseResult.cs` with:

```csharp
using Questline.Framework.Mediator;

namespace Questline.Engine.Parsers;

public class ParseResult
{
    private ParseResult(IRequest? request, Actor? actor, ParseError? error, bool isSuccess)
    {
        Request   = request;
        Actor     = actor;
        Error     = error;
        IsSuccess = isSuccess;
    }

    public bool        IsSuccess { get; }
    public Actor?      Actor     { get; }
    public IRequest?   Request   { get; }
    public ParseError? Error     { get; }

    public static ParseResult Success(IRequest request, Actor? actor = null) =>
        new(request, actor, null, true);

    public static ParseResult Failure(ParseError error) =>
        new(null, null, error, false);
}
```

- [ ] **Step 10.4: Add `Party?` overload to `Parser.Parse`**

Replace the `Parse` method in `src/Questline/Engine/Parsers/Parser.cs`. Add the new overload that accepts an optional `Party`:

```csharp
public ParseResult Parse(string? input) => Parse(input, party: null);

public ParseResult Parse(string? input, Party? party)
{
    if (string.IsNullOrEmpty(input))
    {
        return ParseResult.Failure(new ParseError("Please enter a command."));
    }

    var tokens = input.Trim().ToLowerInvariant()
        .Split(' ', StringSplitOptions.RemoveEmptyEntries);

    if (tokens.Length == 0)
    {
        return ParseResult.Failure(new ParseError("Please enter a command."));
    }

    Actor actor = new PartyActor();
    var startIndex = 0;

    if (party is not null)
    {
        var match = party.FindByName(tokens[0]);
        if (match is not null)
        {
            actor      = new CharacterActor(match);
            startIndex = 1;
        }
    }

    if (startIndex >= tokens.Length)
    {
        return ParseResult.Failure(new ParseError("Who should do what?"));
    }

    if (_verbToParsers.TryGetValue(tokens[startIndex], out var parser))
    {
        var result = parser(tokens[(startIndex + 1)..]);
        if (!result.IsSuccess)
        {
            return result;
        }
        return ParseResult.Success(result.Request!, actor);
    }

    return ParseResult.Failure(new ParseError($"I don't understand '{tokens[startIndex]}'."));
}
```

Add the necessary usings at the top of the file:

```csharp
using Questline.Domain.Parties.Entity;
using Questline.Framework.Mediator;
```

- [ ] **Step 10.5: Update `GameEngine.HandleGamePlay` to pass party and forward Actor**

In `src/Questline/Engine/Core/GameEngine.cs`, the current `HandleGamePlay`:

```csharp
private async Task<IResponse> HandleGamePlay(string? input)
{
    var parseResult = parser.Parse(input);
    if (!parseResult.IsSuccess)
    {
        return parseResult.Error!;
    }

    var response = await dispatcher.Send(new PartyActor(), parseResult.Request!);

    if (response is Responses.GameQuitedResponse)
    {
        _phase = GamePhase.Ended;
    }

    return response;
}
```

Update to fetch the playthrough's party and forward the Actor from the parse result:

```csharp
private async Task<IResponse> HandleGamePlay(string? input)
{
    var playthrough = await playthroughRepository.GetById(gameSession.PlaythroughId!);
    var parseResult = parser.Parse(input, playthrough.Party);
    if (!parseResult.IsSuccess)
    {
        return parseResult.Error!;
    }

    var response = await dispatcher.Send(parseResult.Actor!, parseResult.Request!);

    if (response is Responses.GameQuitedResponse)
    {
        _phase = GamePhase.Ended;
    }

    return response;
}
```

- [ ] **Step 10.6: Run all tests; expect pass**

```bash
dotnet build && dotnet test --no-build
```

Expected: all green. If `When_parsing_valid_input.cs` fails because the parser's behaviour with a null party diverges, verify that `Parse(input)` (single-arg) still works as before — it should, because it forwards `party: null`.

- [ ] **Step 10.7: Commit**

```bash
git add src/ tests/
git commit -m "Detect actor prefix in Parser; propagate Actor through ParseResult

Parser.Parse gains a Party? overload. When the first token matches a
PC name, the actor is CharacterActor(pc) and the rest of the input is
the command. Otherwise the actor is PartyActor. ParseResult exposes
the Actor; GameEngine.HandleGamePlay forwards it to the dispatcher.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

### Task 11: Honour `RequiresActor` attribute in the parser

**Files:**

- Modify: `src/Questline/Engine/Parsers/Parser.cs`
- Create: `tests/Questline.Tests/Engine/Parsers/Parser/When_required_actor_is_missing.cs`

- [ ] **Step 11.1: Write the failing test**

The parser's verb dictionary is built by scanning `Assembly.GetExecutingAssembly()` (the source assembly), so a test-assembly `[RequiresActor]` request can't be discovered through the normal path. To make the parser testable in isolation, add a constructor overload to `Parser` that accepts a pre-built verb metadata dictionary. The test exercises that overload.

Create `tests/Questline.Tests/Engine/Parsers/Parser/When_required_actor_is_missing.cs`:

```csharp
using Questline.Domain.Parties.Entity;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;

namespace Questline.Tests.Engine.Parsers.Parser;

public class When_required_actor_is_missing
{
    private record FakeActorOnlyRequest : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new FakeActorOnlyRequest();
    }

    [Fact]
    public void Returns_failure_when_no_actor_prefix_present()
    {
        var verbs = new Dictionary<string, (Func<string[], ParseResult> Build, bool RequiresActor)>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["picklock"] = (_ => ParseResult.Success(new FakeActorOnlyRequest()), true)
        };
        var parser = new Questline.Engine.Parsers.Parser(verbs);
        var aric   = CharacterBuilder.New().WithName("Aric").Build();
        var party  = new Party(id: "p", members: [aric]);

        var result = parser.Parse("picklock", party);

        result.IsSuccess.ShouldBeFalse();
        result.Error!.Message.ShouldContain("Which character");
    }

    [Fact]
    public void Succeeds_when_actor_prefix_is_present()
    {
        var verbs = new Dictionary<string, (Func<string[], ParseResult> Build, bool RequiresActor)>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["picklock"] = (_ => ParseResult.Success(new FakeActorOnlyRequest()), true)
        };
        var parser = new Questline.Engine.Parsers.Parser(verbs);
        var aric   = CharacterBuilder.New().WithName("Aric").Build();
        var party  = new Party(id: "p", members: [aric]);

        var result = parser.Parse("aric picklock", party);

        result.IsSuccess.ShouldBeTrue();
        result.Actor.ShouldBeOfType<CharacterActor>();
    }
}
```

These tests require the `Parser` to expose a constructor overload that accepts the verb dictionary directly (a test seam). Add that in the next step.

- [ ] **Step 11.2: Add `RequiresActor` enforcement and test seam to `Parser`**

In `src/Questline/Engine/Parsers/Parser.cs`, change the verb dictionary to a tuple capturing `RequiresActor`, and add a second constructor that accepts the dictionary directly.

Field:

```csharp
private readonly Dictionary<string, (Func<string[], ParseResult> Build, bool RequiresActor)> _verbToParsers;
```

Add a second constructor (the test seam):

```csharp
public Parser(Dictionary<string, (Func<string[], ParseResult> Build, bool RequiresActor)> verbToParsers)
{
    _verbToParsers = verbToParsers;
}
```

Update the existing parameterless constructor to populate the new shape and pass through:

```csharp
public Parser() : this(BuildDefaultVerbDictionary())
{
}

private static Dictionary<string, (Func<string[], ParseResult> Build, bool RequiresActor)> BuildDefaultVerbDictionary()
{
    var verbs = new Dictionary<string, (Func<string[], ParseResult> Build, bool RequiresActor)>(
        StringComparer.OrdinalIgnoreCase);

    var requestTypes = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t => typeof(IRequest).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

    foreach (var type in requestTypes)
    {
        var attr = type.GetCustomAttribute<VerbsAttribute>();
        if (attr == null) continue;

        var requiresActor = type.GetCustomAttribute<RequiresActorAttribute>() != null;

        var createMethod = type.GetMethod(
            "CreateRequest",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        if (createMethod == null) continue;

        var localMethod = createMethod;
        Func<string[], ParseResult> build = args =>
        {
            try
            {
                return localMethod.Invoke(null, [args]) is IRequest request
                    ? ParseResult.Success(request)
                    : ParseResult.Failure(new ParseError("Failed to create request."));
            }
            catch (TargetInvocationException ex)
            {
                var innerException = ex.InnerException ?? ex;
                return ParseResult.Failure(new ParseError($"Invalid arguments: {innerException.Message}"));
            }
            catch (Exception ex)
            {
                return ParseResult.Failure(new ParseError($"Error parsing command: {ex.Message}"));
            }
        };

        foreach (var verb in attr.Verbs)
        {
            verbs[verb] = (build, requiresActor);
        }
    }

    return verbs;
}
```

In `Parse(string?, Party?)`, after a successful verb lookup, enforce `RequiresActor`:

```csharp
if (_verbToParsers.TryGetValue(tokens[startIndex], out var entry))
{
    var result = entry.Build(tokens[(startIndex + 1)..]);
    if (!result.IsSuccess) return result;

    if (entry.RequiresActor && actor is not CharacterActor)
    {
        return ParseResult.Failure(new ParseError("Which character should do that?"));
    }

    return ParseResult.Success(result.Request!, actor);
}
```

- [ ] **Step 11.3: Run all tests; expect pass**

```bash
dotnet build && dotnet test --no-build
```

- [ ] **Step 11.4: Commit**

```bash
git add src/ tests/
git commit -m "Honour [RequiresActor] in Parser

Parser captures the RequiresActor attribute per verb at startup. When
a verb requires an actor and the parsed input has no actor prefix, the
parser returns a 'Which character?' failure. No production verbs carry
the attribute in Phase 1; combat verbs in Phase 2 will.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

---

## Tier 7 — Per-PC handler behaviour

Each handler that touches inventory now routes by `Actor`. Handlers also vary their response wording: `CharacterActor` produces a third-person message with the PC's name; `PartyActor` keeps the existing second-person wording.

### Task 12: Helper for resolving acting character

**Files:**

- Create: `src/Questline/Engine/Core/ActingCharacterResolver.cs`
- Create: `tests/Questline.Tests/Engine/Core/ActingCharacterResolver/When_resolving_actor.cs`

- [ ] **Step 12.1: Write the test**

Create `tests/Questline.Tests/Engine/Core/ActingCharacterResolver/When_resolving_actor.cs`:

```csharp
using Questline.Domain.Parties.Entity;
using Questline.Engine.Core;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;

namespace Questline.Tests.Engine.Core.ActingCharacterResolver;

public class When_resolving_actor
{
    [Fact]
    public void Party_actor_resolves_to_marching_order_leader()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var mira  = CharacterBuilder.New().WithName("Mira").Build();
        var party = new Party(id: "p", members: [aric, mira]);

        var resolved = Questline.Engine.Core.ActingCharacterResolver.Resolve(new PartyActor(), party);

        resolved.ShouldBe(aric);
    }

    [Fact]
    public void Character_actor_resolves_to_named_character()
    {
        var aric  = CharacterBuilder.New().WithName("Aric").Build();
        var mira  = CharacterBuilder.New().WithName("Mira").Build();
        var party = new Party(id: "p", members: [aric, mira]);

        var resolved = Questline.Engine.Core.ActingCharacterResolver.Resolve(new CharacterActor(mira), party);

        resolved.ShouldBe(mira);
    }
}
```

- [ ] **Step 12.2: Implement the resolver**

Create `src/Questline/Engine/Core/ActingCharacterResolver.cs`:

```csharp
using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Framework.Mediator;

namespace Questline.Engine.Core;

public static class ActingCharacterResolver
{
    public static Character Resolve(Actor actor, Party party) =>
        actor switch
        {
            CharacterActor ca => ca.Character,
            PartyActor _      => party.Members[0],
            NoActor _         => throw new InvalidOperationException(
                "NoActor cannot be resolved to a character."),
            _                 => throw new ArgumentOutOfRangeException(nameof(actor))
        };
}
```

- [ ] **Step 12.3: Run tests; expect pass**

```bash
dotnet test --no-build --filter "FullyQualifiedName~ActingCharacterResolver"
```

- [ ] **Step 12.4: Commit**

```bash
git add src/ tests/
git commit -m "Add ActingCharacterResolver helper

PartyActor → leader (Members[0]). CharacterActor → named PC.
NoActor throws (system commands like login don't act on a character).

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

### Task 13: Reroute inventory handlers to acting character

**Files:**

- Modify: `src/Questline/Engine/Handlers/TakeItemHandler.cs`
- Modify: `src/Questline/Engine/Handlers/DropItemCommandHandler.cs`
- Modify: `src/Questline/Engine/Handlers/UseItemCommandHandler.cs`
- Modify: `src/Questline/Engine/Handlers/ExamineCommandHandler.cs`
- Modify: `src/Questline/Engine/Handlers/GetPlayerInventoryQueryHandler.cs`
- Modify: `src/Questline/Engine/Messages/Responses.cs` (per-PC variants of existing item responses)
- Modify: handler tests under `tests/Questline.Tests/Engine/Handlers/`

- [ ] **Step 13.1: Update `Responses.cs` with actor-flavoured variants**

Read `src/Questline/Engine/Messages/Responses.cs` first. Add variants of `ItemTakenResponse`, `ItemDroppedResponse`, etc. that take an optional acting character name. Sketch:

```csharp
public record ItemTakenResponse(string ItemName, string? CharacterName = null) : IResponse
{
    public string Message => CharacterName is null
        ? $"You pick up the {ItemName}."
        : $"{CharacterName} picks up the {ItemName}.";
}
```

Apply the same pattern (optional `CharacterName`) to:
- `ItemTakenResponse`
- `ItemDroppedResponse`
- `ItemExaminedResponse` (if exists)
- Any other response where wording should differ between party and character actors.

- [ ] **Step 13.2: Update `TakeItemHandler` to use the resolver**

Replace the body of `TakeItemHandler.Handle` in `src/Questline/Engine/Handlers/TakeItemHandler.cs`:

```csharp
public async Task<IResponse> Handle(Actor actor, Requests.TakeItemCommand request)
{
    var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);
    var room        = await roomRepository.GetById(playthrough.Location);

    var actingCharacter = ActingCharacterResolver.Resolve(actor, playthrough.Party);

    var roomItems = playthrough.GetRecordedRoomItems(room.Id) ?? room.Items.ToList();
    var item      = roomItems.FirstOrDefault(i =>
        i.Name.Equals(request.ItemName, StringComparison.OrdinalIgnoreCase));

    if (item is null)
    {
        return new ErrorResponse($"There is no '{request.ItemName}' here.");
    }

    roomItems.Remove(item);
    playthrough.RecordRoomItems(room.Id, roomItems);
    actingCharacter.AddInventoryItem(item);
    await playthroughRepository.Save(playthrough);

    var characterName = actor is CharacterActor ? actingCharacter.Name : null;
    return new Responses.ItemTakenResponse(item.Name, characterName);
}
```

Add the necessary usings: `Questline.Engine.Core;` (for `ActingCharacterResolver`), `Questline.Framework.Mediator;` (for `Actor`).

- [ ] **Step 13.3: Update `DropItemCommandHandler` analogously**

Apply the same pattern: resolve the acting character, drop from `actingCharacter.Inventory`, return a response with optional `CharacterName`.

- [ ] **Step 13.4: Update `UseItemCommandHandler` analogously**

Resolve the acting character; the item to use is found in `actingCharacter.Inventory`.

- [ ] **Step 13.5: Update `ExamineCommandHandler`**

Examine looks at three sources: room items, room features, and inventory items. The inventory check uses the acting character's inventory.

- [ ] **Step 13.6: Update `GetPlayerInventoryQueryHandler`**

The query now lists the **whole party's** inventory by default (when `actor is PartyActor`), grouped by character. When `actor is CharacterActor`, it lists only that character's inventory.

Sketch:

```csharp
public async Task<IResponse> Handle(Actor actor, Requests.GetPlayerInventoryQuery request)
{
    var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);

    if (actor is CharacterActor characterActor)
    {
        return new Responses.InventoryResponse(
            new[] { (characterActor.Character.Name, characterActor.Character.Inventory.Select(i => i.Name).ToList()) });
    }

    var partyInventory = playthrough.Party.Members
        .Select(c => (c.Name, c.Inventory.Select(i => i.Name).ToList()))
        .ToList();

    return new Responses.InventoryResponse(partyInventory);
}
```

(Adjust to whatever shape `InventoryResponse` actually takes; expand the response to carry the per-character inventory list if needed.)

- [ ] **Step 13.7: Update handler tests**

For each handler test under `tests/Questline.Tests/Engine/Handlers/`:

- Add a test variant that passes `new CharacterActor(specificPc)` and asserts the response message names that PC.
- Existing tests passing `new PartyActor()` continue to assert second-person wording.

Example new test for `TakeItemHandler/When_actor_is_a_specific_character.cs`:

```csharp
[Fact]
public async Task Item_is_added_to_named_character_inventory()
{
    // Arrange a playthrough with a 2-PC party; the second PC takes the item.
    var aric  = CharacterBuilder.New().WithName("Aric").Build();
    var mira  = CharacterBuilder.New().WithName("Mira").Build();
    // ... build playthrough, repository, room, etc.

    var response = await handler.Handle(new CharacterActor(mira), new TakeItemCommand("rusty key"));

    mira.Inventory.ShouldContain(i => i.Name == "rusty key");
    aric.Inventory.ShouldBeEmpty();
    response.ShouldBeOfType<ItemTakenResponse>();
    ((ItemTakenResponse)response).Message.ShouldContain("Mira picks up");
}
```

- [ ] **Step 13.8: Run all tests; expect pass**

```bash
dotnet build && dotnet test --no-build
```

- [ ] **Step 13.9: Commit**

```bash
git add src/ tests/
git commit -m "Route inventory handlers by Actor

TakeItem, DropItem, UseItem, Examine, and GetPlayerInventory handlers
resolve the acting character via ActingCharacterResolver: PartyActor
defaults to the marching-order leader; CharacterActor uses the named
PC. Response wording is third-person ('Aric picks up the X') for
CharacterActor and second-person ('You pick up the X') for PartyActor.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

### Task 14: Remove leader-shorthand methods from `Playthrough`

After Task 13 no handler uses `Playthrough.AddInventoryItem` etc. — they call the character directly. The shorthand methods can be removed.

**Files:**

- Modify: `src/Questline/Domain/Playthroughs/Entity/Playthrough.cs`
- Modify: any remaining test that uses the shorthand

- [ ] **Step 14.1: Verify no production code uses the shorthand**

```bash
git grep -nE "playthrough\.(AddInventoryItem|RemoveInventoryItem|FindInventoryItemByName|Inventory)\b" src/
```

Expected: empty result for `src/`. If anything still uses them, update it to navigate via `playthrough.Party.Members[i].Inventory` etc.

- [ ] **Step 14.2: Remove the shorthand methods**

In `src/Questline/Domain/Playthroughs/Entity/Playthrough.cs`, remove:

```csharp
public IReadOnlyList<Item> Inventory => Party.Members[0].Inventory;
public void AddInventoryItem(Item item) => Party.Members[0].AddInventoryItem(item);
public void RemoveInventoryItem(Item item) => Party.Members[0].RemoveInventoryItem(item);
public Item? FindInventoryItemByName(string name) => Party.Members[0].FindInventoryItemByName(name);
```

- [ ] **Step 14.3: Update remaining tests**

Tests that previously called the shorthand on `Playthrough` should now navigate to `playthrough.Party.Members[0]`. Specific files:

- `tests/Questline.Tests/Domain/Playthroughs/Entity/Playthrough/When_managing_inventory.cs` — rename to live under Character (its tests now belong to `Character.When_managing_inventory` from Task 8). If duplicative, delete this file.

- [ ] **Step 14.4: Run all tests; expect pass**

```bash
dotnet build && dotnet test --no-build
```

- [ ] **Step 14.5: Commit**

```bash
git add src/ tests/
git commit -m "Remove Playthrough leader-shorthand inventory methods

Now that handlers route inventory operations through ActingCharacterResolver,
the temporary delegation methods on Playthrough are dead code. Tests
that asserted leader inventory semantics already moved to Character.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

---

## Tier 8 — Turn counter

### Task 15: Increment `Turns` on each accepted command and surface in stats

**Files:**

- Modify: `src/Questline/Engine/Core/GameEngine.cs` (`HandleGamePlay`)
- Modify: `src/Questline/Engine/Handlers/...` (`stats` command — see below)
- Modify: `src/Questline/Engine/Messages/Requests.cs` and `Responses.cs` if `stats` is new
- Create: `tests/Questline.Tests/Engine/Core/GameEngine/When_incrementing_turn_counter.cs`

The current codebase doesn't have a `stats` command (the existing flow shows character info via responses on adventure start). For Phase 1, add a `StatsQuery` request + handler.

- [ ] **Step 15.1: Verify whether `stats` already exists**

```bash
git grep -nE "Stats|stats" src/Questline/Engine/Messages/
```

If a `StatsQuery` or similar exists, modify the existing handler. Otherwise create new ones (see steps below).

- [ ] **Step 15.2: Write a unit test for `Playthrough.IncrementTurns`**

Create `tests/Questline.Tests/Domain/Playthroughs/Entity/Playthrough/When_incrementing_turns.cs`:

```csharp
using Questline.Tests.TestHelpers.Builders;
using Shouldly;

namespace Questline.Tests.Domain.Playthroughs.Entity.Playthrough;

public class When_incrementing_turns
{
    [Fact]
    public void Starts_at_zero()
    {
        var playthrough = PlaythroughBuilder.New().Build();

        playthrough.Turns.ShouldBe(0);
    }

    [Fact]
    public void Each_increment_advances_by_one()
    {
        var playthrough = PlaythroughBuilder.New().Build();

        playthrough.IncrementTurns();
        playthrough.IncrementTurns();
        playthrough.IncrementTurns();

        playthrough.Turns.ShouldBe(3);
    }
}
```

Also add an integration-style test on `GameEngine` modelled on existing tests in `tests/Questline.Tests/Engine/Core/GameEngine/`. Read `When_loading_game.cs` as a template; the pattern is: build a `GameEngine` via the existing `GameEngineBuilder` (or fixture), drive it through Login → StartMenu → NewAdventure → PartyCreation → Playing, then send three valid commands and assert the saved playthrough's `Turns` is 3. If the existing tests don't expose a way to inspect the saved playthrough, instead assert the `stats` response contains "Turns: 3".

- [ ] **Step 15.3: Increment the turn counter in `HandleGamePlay`**

In `src/Questline/Engine/Core/GameEngine.cs`, modify `HandleGamePlay`:

```csharp
private async Task<IResponse> HandleGamePlay(string? input)
{
    var playthrough = await playthroughRepository.GetById(gameSession.PlaythroughId!);
    var parseResult = parser.Parse(input, playthrough.Party);
    if (!parseResult.IsSuccess)
    {
        return parseResult.Error!;
    }

    var response = await dispatcher.Send(parseResult.Actor!, parseResult.Request!);

    playthrough.IncrementTurns();
    await playthroughRepository.Save(playthrough);

    if (response is Responses.GameQuitedResponse)
    {
        _phase = GamePhase.Ended;
    }

    return response;
}
```

The increment + save happens after a successful dispatch. Saves are idempotent.

Note: handlers also save the playthrough internally. The double save isn't a problem (last-write-wins on Mongo). To avoid it, we could skip the save here when the response carries a flag — but for Phase 1, two saves per command is acceptable.

- [ ] **Step 15.4: Add `StatsQuery` request, response, and handler**

Add to `src/Questline/Engine/Messages/Requests.cs`:

```csharp
[Verbs("stats", "status")]
public record StatsQuery : IRequest
{
    public static IRequest CreateRequest(string[] args) => new StatsQuery();
}
```

Add a response in `src/Questline/Engine/Messages/Responses.cs`:

```csharp
public record StatsResponse(PartySummary Party) : IResponse
{
    public string Message
    {
        get
        {
            var lines = Party.Members.Select(m =>
                $"{m.Name} ({m.Race}, {m.Class}) — HP {m.CurrentHitPoints}/{m.MaxHitPoints}, " +
                $"Str {m.AbilityScores.Strength} Dex {m.AbilityScores.Dexterity} " +
                $"Con {m.AbilityScores.Constitution} Int {m.AbilityScores.Intelligence} " +
                $"Wis {m.AbilityScores.Wisdom} Cha {m.AbilityScores.Charisma}");

            return string.Join(Environment.NewLine, lines)
                + Environment.NewLine
                + $"Turns: {Party.Turns}";
        }
    }
}
```

Adjust `CharacterSummary` if its property names differ — the goal is one summary line per PC plus a final `Turns: <n>` line.

Create `src/Questline/Engine/Handlers/StatsQueryHandler.cs`:

```csharp
using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class StatsQueryHandler(
    IGameSession           session,
    IPlaythroughRepository playthroughRepository) : IRequestHandler<Requests.StatsQuery>
{
    public async Task<IResponse> Handle(Actor actor, Requests.StatsQuery request)
    {
        var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);
        return new Responses.StatsResponse(playthrough.ToPartySummary());
    }
}
```

Register the handler in `src/Questline/Engine/ServiceCollectionExtensions.cs`:

```csharp
services.AddSingleton<IRequestHandler<StatsQuery>, StatsQueryHandler>();
```

(Add `using static Questline.Engine.Messages.Requests;` if not already there.)

- [ ] **Step 15.5: Run all tests; expect pass**

```bash
dotnet build && dotnet test --no-build
```

- [ ] **Step 15.6: Commit**

```bash
git add src/ tests/
git commit -m "Add turn counter; expose via stats command

Playthrough.Turns increments after each successfully dispatched
command in Playing phase. New StatsQuery (verbs: 'stats', 'status')
returns each PC's summary plus the turn count.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

---

## Tier 9 — Party Creation flow

### Task 16: Phase-1 occupation list and per-race name tables

**Files:**

- Create: `src/Questline/Engine/Characters/PartyGeneration/Occupations.cs`
- Create: `src/Questline/Engine/Characters/PartyGeneration/Names.cs`

- [ ] **Step 16.1: Define the occupation list**

Create `src/Questline/Engine/Characters/PartyGeneration/Occupations.cs`:

```csharp
namespace Questline.Engine.Characters.PartyGeneration;

public static class Occupations
{
    public static readonly IReadOnlyList<string> All =
    [
        "Apprentice scribe",
        "Beekeeper",
        "Charcoal-burner",
        "Cooper",
        "Cordwainer",
        "Dyer",
        "Falconer's hand",
        "Fishmonger",
        "Goose-girl",
        "Herbalist",
        "Hireling porter",
        "Inn-keeper",
        "Lamplighter",
        "Mason's apprentice",
        "Miller",
        "Outlaw",
        "Pilgrim",
        "Smithy boy",
        "Tinker",
        "Wandering minstrel"
    ];

    public static string Pick(IDice dice) =>
        All[dice.Roll(All.Count) - 1];
}
```

- [ ] **Step 16.2: Define the name tables**

Create `src/Questline/Engine/Characters/PartyGeneration/Names.cs`:

```csharp
using Questline.Domain.Characters.Entity;

namespace Questline.Engine.Characters.PartyGeneration;

public static class Names
{
    private static readonly IReadOnlyList<string> Human =
        ["Aric", "Borin", "Cedric", "Dunwald", "Edric", "Faelan", "Galen", "Hadric",
         "Mira", "Niamh", "Orla", "Petra", "Rosa", "Sela", "Talia", "Una"];

    private static readonly IReadOnlyList<string> Dwarf =
        ["Brokk", "Durin", "Falin", "Garr", "Korin", "Morrick", "Nain", "Thrain",
         "Brida", "Dagna", "Hilda", "Mara", "Rilda", "Sigrun", "Thora", "Vala"];

    private static readonly IReadOnlyList<string> Elf =
        ["Aelric", "Belion", "Caelan", "Daerion", "Elond", "Faerion", "Galin", "Lirael",
         "Aelyn", "Belira", "Caelith", "Erendel", "Larael", "Mirien", "Saelin", "Tarael"];

    private static readonly IReadOnlyList<string> Halfling =
        ["Bilbo", "Cosmo", "Drogo", "Frodo", "Gilly", "Hobson", "Otho", "Pipin",
         "Belba", "Cora", "Daisy", "Esmera", "Lily", "Marigold", "Pansy", "Ruby"];

    public static string Pick(Race race, IDice dice)
    {
        var pool = race switch
        {
            Race.Human    => Human,
            Race.Dwarf    => Dwarf,
            Race.Elf      => Elf,
            Race.Halfling => Halfling,
            _             => Human
        };
        return pool[dice.Roll(pool.Count) - 1];
    }
}
```

- [ ] **Step 16.3: Verify build**

```bash
dotnet build
```

- [ ] **Step 16.4: Commit**

```bash
git add src/
git commit -m "Add Phase-1 occupation list and per-race name tables

Twenty flavour occupations and per-race name pools (16 each for the
four races). Pick() picks one entry using the supplied IDice. Used by
PartyCreationStateMachine in the next task.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

### Task 17: Implement `PartyCreationStateMachine`

**Files:**

- Create: `src/Questline/Engine/Characters/PartyCreationStateMachine.cs`
- Modify: `src/Questline/Engine/Messages/Responses.cs` (party-creation responses)
- Create: `tests/Questline.Tests/Engine/Characters/PartyCreationStateMachine/When_rolling_initial_party.cs`
- Create: `tests/Questline.Tests/Engine/Characters/PartyCreationStateMachine/When_rerolling.cs`
- Create: `tests/Questline.Tests/Engine/Characters/PartyCreationStateMachine/When_renaming_a_character_by_slot.cs`
- Create: `tests/Questline.Tests/Engine/Characters/PartyCreationStateMachine/When_renaming_a_character_by_name.cs`

- [ ] **Step 17.1: Add party-creation responses**

In `src/Questline/Engine/Messages/Responses.cs`, add:

```csharp
public record PartyRolledResponse(IReadOnlyList<CharacterSummary> Members) : IResponse
{
    public string Message
    {
        get
        {
            var lines = Members.Select((m, i) =>
                $"{i + 1}. {m.Name} ({m.Race}, {m.Occupation}) — " +
                $"HP {m.CurrentHitPoints}/{m.MaxHitPoints}, " +
                $"Str {m.AbilityScores.Strength}/Dex {m.AbilityScores.Dexterity}/" +
                $"Con {m.AbilityScores.Constitution}/Int {m.AbilityScores.Intelligence}/" +
                $"Wis {m.AbilityScores.Wisdom}/Cha {m.AbilityScores.Charisma}");

            return "Your party of hopefuls:" + Environment.NewLine
                + string.Join(Environment.NewLine, lines) + Environment.NewLine
                + Environment.NewLine
                + "Type 'accept' to begin, 'reroll' to start over, or "
                + "'rename <slot> <name>' to rename a character.";
        }
    }
}

public record PartyAcceptedResponse : IResponse { public string Message => "The party sets out…"; }

public record PartyCreationErrorResponse(string Reason) : IResponse
{
    public string Message => Reason;
}
```

`CharacterSummary` needs an `Occupation` field — add it. In `src/Questline/Domain/Characters/Data/CharacterSummary.cs`:

```csharp
public record CharacterSummary(
    string Name,
    string Race,
    string Class,
    string Occupation,
    int    Level,
    int    Experience,
    int    MaxHitPoints,
    int    CurrentHitPoints,
    AbilityScoresSummary AbilityScores);
```

Update `Character.ToSummary()` in `src/Questline/Domain/Characters/Entity/Character.cs` to pass `Occupation`.

- [ ] **Step 17.2: Implement `PartyCreationStateMachine`**

Create `src/Questline/Engine/Characters/PartyCreationStateMachine.cs`:

```csharp
using System.Reflection;
using Questline.Domain.Characters;
using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Engine.Characters.PartyGeneration;
using Questline.Engine.Messages;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;

namespace Questline.Engine.Characters;

public class PartyCreationStateMachine(IDice dice)
{
    private const int PartySize = 4;

    private Party? _party;

    public Party? CompletedParty { get; private set; }

    public IResponse Start()
    {
        _party = RollParty();
        return BuildRolledResponse();
    }

    public IResponse ProcessInput(string? input)
    {
        if (_party is null)
        {
            return Start();
        }

        var trimmed = input?.Trim() ?? "";

        if (string.Equals(trimmed, "accept", StringComparison.OrdinalIgnoreCase))
        {
            CompletedParty = _party;
            return new Responses.PartyAcceptedResponse();
        }

        if (string.Equals(trimmed, "reroll", StringComparison.OrdinalIgnoreCase))
        {
            _party = RollParty();
            return BuildRolledResponse();
        }

        if (trimmed.StartsWith("rename ", StringComparison.OrdinalIgnoreCase))
        {
            return HandleRename(trimmed["rename ".Length..]);
        }

        return new Responses.PartyCreationErrorResponse(
            "Type 'accept', 'reroll', or 'rename <slot|name> <newName>'.");
    }

    private IResponse HandleRename(string args)
    {
        var parts = args.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return new Responses.PartyCreationErrorResponse(
                "Usage: rename <slot|name> <newName>");
        }

        var target  = parts[0];
        var newName = parts[1];

        if (!IsValidName(newName, _party!))
        {
            return new Responses.PartyCreationErrorResponse(
                $"Invalid name '{newName}': must be a single token, unique, and not a reserved verb.");
        }

        var index = ResolveTargetIndex(target);
        if (index is null)
        {
            return new Responses.PartyCreationErrorResponse($"No character matches '{target}'.");
        }

        var renamed = ReplaceMemberName(_party!, index.Value, newName);
        _party = renamed;
        return BuildRolledResponse();
    }

    private int? ResolveTargetIndex(string target)
    {
        if (int.TryParse(target, out var slot) && slot is >= 1 and <= PartySize)
        {
            return slot - 1;
        }

        for (var i = 0; i < _party!.Members.Count; i++)
        {
            if (_party.Members[i].Name.Equals(target, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }
        return null;
    }

    private static Party ReplaceMemberName(Party party, int index, string newName)
    {
        var members = party.Members.ToList();
        var old     = members[index];
        members[index] = Character.Create(
            id: old.Id,
            name: newName,
            race: old.Race,
            characterClass: old.Class,
            hitPoints: old.HitPoints,
            abilityScores: old.AbilityScores,
            occupation: old.Occupation,
            level: old.Level);

        // Restore inventory items if any (none in PartyCreation, but safe).
        foreach (var item in old.Inventory)
        {
            members[index].AddInventoryItem(item);
        }

        return new Party(party.Id, members);
    }

    private Party RollParty()
    {
        var members = new List<Character>(PartySize);

        for (var i = 0; i < PartySize; i++)
        {
            var race          = PickRace(dice);
            var abilityScores = AbilityScoresCalculator.Calculate(dice);
            var hp            = dice.Roll(1, 4);
            var hitPoints     = new HitPoints(max: hp, current: hp);
            var occupation    = Occupations.Pick(dice);

            string name;
            do
            {
                name = Names.Pick(race, dice);
            }
            while (NameClashes(name, members));

            members.Add(Character.Create(
                id: Guid.NewGuid().ToString(),
                name: name,
                race: race,
                characterClass: null,
                hitPoints: hitPoints,
                abilityScores: abilityScores,
                occupation: occupation,
                level: 0));
        }

        return new Party(id: Guid.NewGuid().ToString(), members: members);
    }

    private static Race PickRace(IDice dice) =>
        (Race)dice.Roll(4); // 1..4 maps to Human/Dwarf/Elf/Halfling order in the enum

    private bool IsValidName(string name, Party currentParty)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        if (name.Contains(' ')) return false;
        if (ReservedVerbs.Contains(name.ToLowerInvariant())) return false;
        if (currentParty.Members.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return false;
        return true;
    }

    private static bool NameClashes(string name, IReadOnlyList<Character> members)
    {
        if (ReservedVerbs.Contains(name.ToLowerInvariant())) return true;
        return members.Any(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    private static readonly HashSet<string> ReservedVerbs = BuildReservedVerbs();

    private static HashSet<string> BuildReservedVerbs()
    {
        var verbs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var type in typeof(Requests).Assembly.GetTypes())
        {
            var attr = type.GetCustomAttribute<VerbsAttribute>();
            if (attr is null) continue;
            foreach (var v in attr.Verbs) verbs.Add(v);
        }
        return verbs;
    }

    private IResponse BuildRolledResponse() =>
        new Responses.PartyRolledResponse(
            _party!.Members.Select(m => m.ToSummary()).ToList());
}
```

This compiles only after `Race` enum order is verified. In `src/Questline/Domain/Characters/Entity/Race.cs`, the enum probably has `Human` as the first value. Verify (or set explicit values):

```csharp
public enum Race { Human = 1, Dwarf = 2, Elf = 3, Halfling = 4 }
```

Use explicit values to keep `PickRace` predictable.

- [ ] **Step 17.3: Write tests**

Create `tests/Questline.Tests/Engine/Characters/PartyCreationStateMachine/When_rolling_initial_party.cs`:

```csharp
using Questline.Engine.Characters;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers;
using Shouldly;

namespace Questline.Tests.Engine.Characters.PartyCreationStateMachine;

public class When_rolling_initial_party
{
    [Fact]
    public void Rolls_four_level_zero_characters()
    {
        // 4 PCs * (1 race + 6 ability scores + 1 hp + 1 occupation + 1 name) = 40 die slots minimum.
        // Use deterministic values to avoid name-collision rerolls.
        var diceValues = ProduceDeterministicRolls(party: 4);
        var dice = new FakeDice(diceValues);

        var sm = new Questline.Engine.Characters.PartyCreationStateMachine(dice);
        var response = sm.Start();

        response.ShouldBeOfType<Responses.PartyRolledResponse>();
        var rolled = (Responses.PartyRolledResponse)response;
        rolled.Members.Count.ShouldBe(4);
        rolled.Members.ShouldAllBe(m => m.Level == 0);
        rolled.Members.ShouldAllBe(m => m.Class == "Level 0");
    }

    private static int[] ProduceDeterministicRolls(int party)
    {
        // For each PC: race(1), 6 ability scores (each 3d6 = 18 die slots), hp (1 slot),
        // occupation (1 slot), name (1 slot) = 22 slots per PC. Build values that are
        // safe for index lookups (occupation index <= 20, name index <= 16).
        var rolls = new List<int>();
        for (var i = 0; i < party; i++)
        {
            rolls.Add(1);                 // race: Human
            for (var s = 0; s < 18; s++) rolls.Add(3); // 6 ability scores × 3d6, each die = 3
            rolls.Add(2);                 // hp = 2
            rolls.Add(1);                 // occupation: first entry
            rolls.Add(i + 1);             // name: nth entry (unique)
        }
        return rolls.ToArray();
    }
}
```

Adjust the slot counts if the actual `RollParty` ordering differs.

Create the other three test files using the same `FakeDice` strategy:
- `When_rerolling.cs` — call `Start()`, then `ProcessInput("reroll")`; verify a fresh party is produced.
- `When_renaming_a_character_by_slot.cs` — `ProcessInput("rename 2 Borin")`; verify member 2 has name "Borin".
- `When_renaming_a_character_by_name.cs` — same but using current name.

- [ ] **Step 17.4: Run tests; expect pass**

```bash
dotnet build && dotnet test --no-build --filter "FullyQualifiedName~PartyCreationStateMachine"
```

- [ ] **Step 17.5: Commit**

```bash
git add src/ tests/
git commit -m "Add PartyCreationStateMachine

Rolls a party of four level-0 characters (random race, 3d6 in order
ability scores, 1d4 HP, random occupation, unique random name).
Accepts 'accept', 'reroll', and 'rename <slot|name> <newName>'.
Names are validated unique within the party and not colliding with
reserved verbs.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

### Task 18: Replace `CharacterCreation` phase with `PartyCreation` in `GameEngine`

**Files:**

- Modify: `src/Questline/Engine/Core/GameEngine.cs`
- Modify: `src/Questline/Engine/ServiceCollectionExtensions.cs`
- Delete: `src/Questline/Engine/Characters/CharacterCreationStateMachine.cs`
- Delete: `tests/Questline.Tests/Engine/Characters/CharacterCreationStateMachine/`
- Modify: `tests/Questline.Tests/Engine/Core/GameEngine/` tests

- [ ] **Step 18.1: Rename the phase enum value**

In `src/Questline/Engine/Core/GameEngine.cs`, rename `GamePhase.CharacterCreation` to `GamePhase.PartyCreation` (find/replace across the file and any tests).

- [ ] **Step 18.2: Replace the `HandleCharacterCreation` method body**

Inject the new state machine into the `GameEngine` constructor:

```csharp
public class GameEngine(
    Parser                       parser,
    RequestSender                dispatcher,
    IAdventureRepository         adventureRepository,
    IRoomRepository              roomRepository,
    IPlaythroughRepository       playthroughRepository,
    IGameSession                 gameSession,
    PartyCreationStateMachine    stateMachine)
```

Replace `HandleCharacterCreation` with `HandlePartyCreation`:

```csharp
private async Task<IResponse> HandlePartyCreation(string? input)
{
    var response = stateMachine.ProcessInput(input);

    if (stateMachine.CompletedParty is { } party)
    {
        var adventure   = await adventureRepository.GetById(_selectedAdventureId);
        var playthrough = Playthrough.Create(
            gameSession.Username!,
            _selectedAdventureId,
            adventure.StartingRoomId,
            party);

        await playthroughRepository.Save(playthrough);
        gameSession.SetPlaythroughId(playthrough.Id);

        _phase = GamePhase.Playing;
        return await StartAdventure(playthrough);
    }

    return response;
}
```

Update the `ProcessInput` switch statement:

```csharp
case GamePhase.PartyCreation:
    return await HandlePartyCreation(input);
```

Also update `HandleNewAdventure` to transition to `PartyCreation`:

```csharp
_phase = GamePhase.PartyCreation;
```

Initial `Start()` call: `PartyCreationStateMachine` needs to roll the party on entry. The existing `CharacterCreationStateMachine` started immediately. Mirror that — call `stateMachine.Start()` in `HandlePartyCreation` when `_party is null`. (The `ProcessInput` method on the new state machine already handles this via the null check.)

- [ ] **Step 18.3: Update DI registration**

In `src/Questline/Engine/ServiceCollectionExtensions.cs`, replace:

```csharp
services.AddSingleton<CharacterCreationStateMachine>();
```

with:

```csharp
services.AddTransient<PartyCreationStateMachine>();
```

(`Transient` because each playthrough should get a fresh state machine; if `GameEngine` is `Singleton`, the resolution boundary needs care — verify the existing scope.)

- [ ] **Step 18.4: Delete `CharacterCreationStateMachine` and its tests**

```bash
git rm src/Questline/Engine/Characters/CharacterCreationStateMachine.cs
git rm -r tests/Questline.Tests/Engine/Characters/CharacterCreationStateMachine/
```

Also remove `Responses.CharacterCreationResponse`, `Responses.CharacterCreationCompleteResponse`, `Responses.CharacterCreationStep`, `Responses.CharacterCreationOption` from `src/Questline/Engine/Messages/Responses.cs` (no longer used).

- [ ] **Step 18.5: Update `GameEngine` tests**

Tests in `tests/Questline.Tests/Engine/Core/GameEngine/` may reference `CharacterCreation` phase or character-creation responses. Update them:

- `When_loading_game.cs`, `When_logging_in.cs`, `When_on_start_menu.cs` — these primarily test login/load flow and shouldn't change much.
- Any test that walks through character creation needs to walk through party creation instead. Use `FakeDice` with deterministic values and `accept` as the input.

Also: `tests/Questline.Tests/Cli/Game/GameApp/When_playing_game.cs` likely walks an end-to-end flow. Update its inputs to drive the party-creation flow.

- [ ] **Step 18.6: Run all tests; expect pass**

```bash
dotnet build && dotnet test --no-build
```

- [ ] **Step 18.7: Commit**

```bash
git add -A src/ tests/
git commit -m "Replace CharacterCreation phase with PartyCreation

GameEngine now drives PartyCreationStateMachine instead of the single-
character CharacterCreationStateMachine. The latter is deleted along
with its tests and now-unused response types. NewAdventure transitions
into PartyCreation; on accept, a Playthrough is created with the rolled
Party and the game enters Playing phase.

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
```

---

## Tier 10 — End-to-end smoke test

### Task 19: Drop and reseed; smoke-test the full flow

This task validates the system end-to-end manually. It produces no commit unless data fixtures need updating.

**Pre-requisites:** Aspire app running (`aspire run`).

- [ ] **Step 19.1: Drop existing dev data**

The simplest way is via the Aspire dashboard or `mongosh`:

```bash
mongosh "mongodb://localhost:27017/questline" --eval 'db.dropDatabase()'
```

(Confirm host/port from your Aspire setup. Adjust if necessary.)

- [ ] **Step 19.2: Run the content deployer**

```bash
dotnet run --project src/Questline -- --mode deploy-content
```

Expected: rooms and adventures from `content/adventures/the-goblins-lair.json` are seeded into Mongo.

- [ ] **Step 19.3: Play through the adventure manually**

```bash
dotnet run --project src/Questline
```

Walk through the flow:

1. Login as a fresh user (`login alice`).
2. Choose `1` for New Game.
3. Choose `1` for "The Goblins' Lair".
4. The party-creation flow rolls four PCs. Try each command:
   - `reroll` — the four PCs change.
   - `rename 1 Aldric` — the first PC gets the new name.
   - `accept` — game starts.
5. In the adventure:
   - `look` — party-level room description.
   - `take rusty key` — leader picks it up.
   - `aric drop rusty key` (use whichever PC name appears in slot 1) — the named PC drops it.
   - `inventory` — shows all four PCs and what each carries.
   - `<pcname> inventory` — shows that PC's inventory only.
   - `stats` — shows all four PCs and the turn counter.
   - `go north`, `use rusty key on iron door`, etc. — the existing puzzle still works.

If anything misbehaves, fix the underlying handler/parser/state-machine and add a regression test.

- [ ] **Step 19.4: Run the full test suite one more time**

```bash
dotnet build && dotnet test --no-build
```

Expected: all green.

- [ ] **Step 19.5: Push the branch and open a PR**

```bash
git push -u origin feature/phase-1-foundation-pivot
gh pr create -a @me -t "Phase 1: Foundation Pivot — party + actor attribution + live stats" --body "$(cat <<'EOF'
## Summary

- Replaces single-character `Playthrough` with a four-PC `Party` (level-0 funnel start, no class yet).
- Introduces action attribution: parser detects an actor prefix; handlers receive an `Actor` (`PartyActor` / `CharacterActor` / `NoActor`).
- Makes HP and ability stats live: `HitPoints` is mutable with `Damage`/`Heal`, `AbilityScore.Modifier` is queryable.
- Adds a generic dice/check primitive: `IDice.Roll` scalar overloads + `Check(modifier, dc)` extension.
- Adds a turn counter, surfaced via the new `stats` / `status` command.
- Replaces single-character creation with a DCC-style level-0 funnel party-creation flow (`accept`/`reroll`/`rename`).

## Test plan

- [ ] `dotnet build && dotnet test --no-build` — all green
- [ ] `aspire run` (in another terminal)
- [ ] Drop the `questline` db, redeploy content with `--mode deploy-content`
- [ ] Login → New Game → The Goblins' Lair
- [ ] Party-creation: `reroll`, `rename 1 <name>`, `accept`
- [ ] Adventure: `look`, `take rusty key`, `<pc> drop rusty key`, `inventory`, `<pc> inventory`, `stats`, `go north`, `use rusty key on iron door`

## Spec & plan

- Spec: `docs/superpowers/specs/2026-04-29-phase-1-foundation-pivot-design.md`
- Plan: `docs/superpowers/plans/2026-04-29-phase-1-foundation-pivot.md`

🤖 Generated with [Claude Code](https://claude.com/claude-code)
EOF
)"
```

---

## Self-review

After completing all tasks, run a final review:

- [ ] **R1: Spec coverage.** Walk through `docs/superpowers/specs/2026-04-29-phase-1-foundation-pivot-design.md` section by section. For each requirement, confirm a task implemented it. (Domain Model → Tasks 7–9; Action Attribution → Tasks 5, 6, 10–13; Live Stats & Dice → Tasks 1–4; Turn Counter → Task 15; Party Creation Flow → Tasks 16–18; Test Strategy → distributed across all tiers.)

- [ ] **R2: All success criteria met.**
  - The Goblins' Lair plays end-to-end with a 4-PC party — verified in Task 19.
  - Existing verbs work as party-level by default — Tasks 6, 13.
  - Actor prefix routes to the named PC — Tasks 10, 13.
  - Turn counter visible via `stats` — Task 15.
  - Party creation deterministic under stub `IDice` — Task 17.
  - All tests green — verified after each task and finally in Task 19.

- [ ] **R3: Updated `docs/roadmap.md`.** Mark Phase 1 complete (`[x]`); set Phase 2 status to "next".

- [ ] **R4: Final commit if any docs changed.**

```bash
git add docs/
git commit -m "Mark Phase 1 complete in roadmap

Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>"
git push
```
