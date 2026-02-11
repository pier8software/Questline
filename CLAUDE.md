# CLAUDE.md

This file provides instructions for Claude Code when working on the Questline project.

## Project Overview

Questline is a parser-driven text adventure engine built in C# (.NET 10). The long-term vision is a cooperative MUD platform, but development follows an incremental roadmap starting with a single-player experience.

**Current Phase:** 0 (Foundation)

The goal of Phase 0 is to build a playable 5-room dungeon that proves the core loop works, using architecture that supports future multiplayer.

## Key Files

| Location | Purpose |
|----------|---------|
| `openspec/config.yaml` | Project context, tech stack, architecture summary, rules |
| `openspec/specs/` | Capability specifications (requirements, scenarios) |
| `src/Questline/` | Main application code |
| `tests/Questline.Tests/` | Test project |
| `content/adventures/` | JSON content files for adventures |

## Development Workflow

### 1. Read the Spec First

Before implementing anything, read the relevant capability spec in `openspec/specs/<capability>/spec.md`. Each spec contains:

- Purpose
- Requirements (SHALL language with WHEN/THEN scenarios)
- Implementation Notes (for planned capabilities)

### 2. Test First, Always

No production code without a failing test.
Write test names as clear, declarative statements that describe behavior from a business perspective.
Use class names to represent the subject and method names to describe scenarios, improving readability and grouping
Example Test Class:

```csharp
public class RoomTests
{
    // Good Test Name
    [Fact]
    public void Next_Room_Loaded_When_Player_Exits_Room() { /* ... */ }

    // Bad Test Name
    [Fact]
    public void GoCommandHandler_CallsRoomRepository_WithCorrectId() { /* ... */ }
}
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

Specs are updated through the OpenSpec change workflow:

1. Create a change with a proposal and delta-spec for any new or modified requirements
2. Implement the change
3. Archive the change (syncs delta-spec into the main spec)

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
- [ ] Changes align with relevant OpenSpec specs

## Updating Specs

Specs live in `openspec/specs/<capability>/spec.md` and are updated through the OpenSpec change workflow — not edited directly.

### Creating a New Spec

Create `openspec/specs/<capability-name>/spec.md` following this format:

```markdown
# <capability-name> Specification

## Purpose

What this capability does and why it matters.

## Requirements

### Requirement: <descriptive name>

The system SHALL <behaviour>.

#### Scenario: <descriptive name>

- **WHEN** <precondition and action>
- **THEN** <expected outcome using SHALL>

## Implementation Notes (planned capabilities only)

Key models, patterns, and design guidance.
```

### Guidelines

- Use SHALL for requirements
- Every requirement must have at least one WHEN/THEN scenario
- Implemented specs: requirements only (code is the source of truth)
- Planned specs: include Implementation Notes with key models and patterns

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

1. Capability spec for the current work (`openspec/specs/<capability>/spec.md`)
2. `openspec/config.yaml` for architecture, conventions, and project context
3. Existing code for precedent
4. Ask for clarification if still uncertain
