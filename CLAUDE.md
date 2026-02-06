# CLAUDE.md

This file provides instructions for Claude Code when working on the Questline project.

## Project Overview

Questline is a parser-driven text adventure engine built in C# (.NET 10). The long-term vision is a cooperative MUD platform, but development follows an incremental roadmap starting with a single-player experience.

**Current Phase:** 0 (Foundation)

The goal of Phase 0 is to build a playable 5-room dungeon that proves the core loop works, using architecture that supports future multiplayer.

## Key Files

| Location | Purpose |
|----------|---------|
| `docs/PRODUCT_PLAN.md` | Mission, goals, roadmap overview |
| `docs/ARCHITECTURE.md` | Component design, namespace responsibilities, design decisions |
| `docs/CODE_STANDARDS.md` | Tech stack, style, testing approach, git workflow |
| `docs/milestones/` | Detailed specs for each milestone with acceptance criteria |
| `src/Questline/` | Main application code |
| `tests/Questline.Tests/` | Test project |
| `content/adventures/` | JSON content files for adventures |

## Development Workflow

### 1. Read the Spec First

Before implementing anything, read the relevant milestone spec in `docs/milestones/`. Each spec contains:

- Objective
- Acceptance criteria (checkboxes)
- Implementation notes with code examples
- Test approach

### 2. Test First, Always

No production code without a failing test. Follow behavioural testing:

```csharp
// Good: tests observable behaviour
[Fact]
public void GoNorth_WhenExitExists_MovesPlayerToDestination()

// Bad: tests implementation details
[Fact]
public void GoCommandHandler_CallsRoomRepository_WithCorrectId()
```

Use Shouldly for assertions:

```csharp
player.Location.ShouldBe("tavern");
result.ShouldBeOfType<ItemPickedUpEvent>();
inventory.Items.ShouldContain(item => item.Name == "lamp");
```

### 3. Implementation Order

For each acceptance criterion:

1. Write a failing test
2. Implement the minimum code to pass
3. Refactor if needed
4. Move to next criterion

### 4. Update the Spec

When a milestone is complete:

1. Check all acceptance criteria boxes
2. Change status from `Not Started` → `In Progress` → `Complete`
3. Note any deviations or decisions made

## Code Patterns

### Namespace Structure

```
Questline.Cli       → Entry point, game loop, terminal I/O
Questline.Domain    → Entities, value objects, game rules (no dependencies)
Questline.Engine    → Parser, command pipeline, handlers
Questline.Framework → Persistence, serialisation, messaging
```

**Critical:** Domain has no dependencies on other namespaces. All dependencies flow inward.

### Command Handler Pattern

Commands flow through: Input → Parser → Command → Handler → Event → Output

```csharp
public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    CommandResult Execute(GameState state, TCommand command);
}
```

Handlers:
- Receive a command object (not raw string input)
- Return a result/event (not write directly to console)
- Are registered with the dispatcher by verb

### Entities vs Value Objects

- **Entities** have identity (Room, Item, Character) — use `required string Id`
- **Value Objects** are compared by value (Direction, DamageRange) — use records

### Content as Data

Define content in JSON, not code. Behaviour that can't be expressed declaratively should be implemented as named handlers referenced from content.

## Before You Commit

- [ ] All tests pass (`dotnet test`)
- [ ] Code builds without warnings (`dotnet build`)
- [ ] No commented-out code
- [ ] No `Console.WriteLine` debugging left behind
- [ ] Acceptance criteria updated in milestone spec

## Updating Specs

### Marking Progress

Edit the milestone spec directly:

```markdown
## Status

Complete  ← Update this

## Acceptance Criteria

- [x] Rooms exist as entities  ← Check completed items
- [x] Rooms have exits
```

### Recording Decisions

If you deviate from the spec or make a significant decision, add a note:

```markdown
## Implementation Notes

...

### Decisions Made

- Used `Dictionary<Direction, Exit>` instead of `Dictionary<Direction, string>` to support barriers from the start.
```

## Creating New Milestone Specs

When starting a new phase or breaking down work into milestones, create spec files following this structure:

### File Location and Naming

```
docs/milestones/phase-{phase}.{milestone}-{short-name}.md
```

Examples:
- `phase-0.1-scaffolding.md`
- `phase-1.1-combat-basics.md`
- `phase-2.3-party-inventory.md`

### Template

Use this template for new milestone specs:

```markdown
# Phase X.Y: [Title]

## Status

Not Started

## Objective

> One or two sentences describing what this milestone achieves and why it matters.

## Acceptance Criteria

### [Category 1]
- [ ] Criterion in testable, behavioural terms
- [ ] Another criterion
- [ ] Criteria should be independently verifiable

### [Category 2]
- [ ] Group related criteria under headings
- [ ] Each criterion maps to one or more tests

## Implementation Notes

### Suggested Order

1. Step one
2. Step two
3. Continue in logical sequence

### [Topic Heading]

Provide guidance, code examples, or design suggestions:

```csharp
// Example code showing patterns to follow
public class Example
{
    // ...
}
```

### [Another Topic]

Continue with relevant implementation guidance.

### Test Approach

```csharp
[Fact]
public void DescriptiveTestName_Scenario_ExpectedOutcome()
{
    // Arrange: setup
    // Act: execute
    // Assert: verify
}
```

Describe what to test and how.

## Dependencies

List any milestones that must be complete before this one:

- Phase X.Y: [Name] — reason needed

## Out of Scope

- Things explicitly not included in this milestone
- Features deferred to later phases
- Edge cases not yet handled

## Decisions Made

_Record significant implementation decisions here as work progresses._
```

### Guidelines for Writing Specs

1. **Acceptance criteria must be testable** — if you can't write a test for it, rewrite it
2. **Be specific about behaviour** — "handles errors gracefully" is too vague; "displays error message when item not found" is testable
3. **Include code examples** — show the patterns, not just describe them
4. **Define scope boundaries** — "Out of Scope" prevents creep and sets expectations
5. **Keep milestones small** — if a milestone has more than 10-15 acceptance criteria, consider splitting it

### Updating PRODUCT_PLAN.md

When adding new phases or milestones, update `docs/PRODUCT_PLAN.md` to reflect the roadmap:

```markdown
### Phase 1: Solo Adventure

| Milestone | Deliverable |
|-----------|-------------|
| 1.1 | Combat system basics |
| 1.2 | Enemy AI and encounters |
| ... | ... |
```

## Common Tasks

### Adding a New Command

1. Create command record in `Engine/Commands/`
2. Create handler in `Engine/Handlers/`
3. Register with dispatcher (verb + aliases)
4. Add tests for success and error cases
5. Update `help` command metadata

### Adding a New Entity

1. Define in `Domain/Entities/`
2. Add JSON serialisation support if persisted
3. Update content schema if defined in adventure files
4. Add to save/load if state must persist

### Adding Content

1. Edit JSON files in `content/adventures/[adventure-name]/`
2. Validate references (room IDs, item IDs)
3. Test by playing through

## Questions?

If unclear on approach, check in this order:

1. Milestone spec for the current work
2. `docs/ARCHITECTURE.md` for design patterns
3. `docs/CODE_STANDARDS.md` for conventions
4. Existing code for precedent
5. Ask for clarification if still uncertain
