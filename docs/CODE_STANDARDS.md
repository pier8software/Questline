# Questline Code Standards

## Tech Stack

| Concern | Choice |
|---------|--------|
| **Runtime** | .NET 10 (LTS) |
| **Solution structure** | Monorepo, single app project with namespace separation |
| **Serialisation** | System.Text.Json |
| **Dependency Injection** | Microsoft.Extensions.DependencyInjection |
| **Testing** | xUnit + Shouldly |
| **Console output** | Console.WriteLine (Phase 0) |

## Project Structure

```
Questline/
├── src/
│   ├── Questline/
│   │   ├── Cli/              # Entry point, game loop, terminal I/O
│   │   ├── Domain/           # Entities, value objects, game rules
│   │   ├── Engine/           # Parser, command pipeline, handlers
│   │   └── Framework/        # Persistence, serialisation, messaging
│   └── Questline.Infrastructure/   # IaC (future phases)
├── tests/
│   └── Questline.Tests/
└── content/
    └── adventures/
        └── five-room-dungeon/
```

## Code Style

### General

- Use `.editorconfig` for formatting rules
- File-scoped namespaces
- Primary constructors where appropriate
- Prefer records for immutable data types
- Prefer explicit types over `var` when type isn't obvious from context

### Naming

- PascalCase for public members, types, namespaces
- camelCase for private fields (no underscore prefix unless configured in .editorconfig)
- Descriptive names over abbreviations

### Dependencies

- Domain namespace has no dependencies on other namespaces
- All dependencies flow inward toward Domain
- Use dependency injection for cross-cutting concerns

## Testing Approach

### Philosophy

- **Test-first**: No production code without a failing test
- **Behavioural testing**: Test observable outcomes, not implementation details
- **Integration over mocking**: Use real implementations where practical

### What to Test

Test that:
- Commands produce expected events
- State changes are persisted correctly
- Invalid input is handled gracefully
- Game rules are enforced

Do not test:
- Private method internals
- Framework code (Microsoft DI, System.Text.Json)

### Test Structure

```csharp
public class GoCommandTests
{
    [Fact]
    public void GoNorth_WhenExitExists_MovesPlayerToDestination()
    {
        // Arrange
        var world = new WorldBuilder()
            .WithRoom("start", r => r.WithExit(Direction.North, "destination"))
            .WithRoom("destination")
            .Build();
        var player = new Player { Location = "start" };
        var handler = new GoCommandHandler(world);

        // Act
        var result = handler.Execute(player, new GoCommand(Direction.North));

        // Assert
        player.Location.ShouldBe("destination");
        result.ShouldBeOfType<MovedEvent>();
    }
}
```

### Assertions

Use Shouldly for readable assertions:

```csharp
// Good
player.Location.ShouldBe("tavern");
inventory.Items.ShouldContain(item => item.Name == "lamp");
result.ShouldBeOfType<ItemPickedUpEvent>();

// Avoid
Assert.Equal("tavern", player.Location);
```

## Git Workflow

### Branching

- **Trunk-based development** with short-lived feature branches
- Branch from `main`, merge back to `main`
- No branch naming prefix convention — use descriptive names: `add-inventory-command`, `fix-parser-crash`

### Commits

- Rebase only, no merge commits
- Squash commits when merging PR
- Short summary commit messages, no convention required

### Pull Requests

- All changes via PR
- Require approval before merge
- CI must pass (build + tests)

## CI/CD

### GitHub Actions Pipeline

**Phase 0:**
- Build
- Run tests

**Future phases:**
- Deployment steps

### Branch Protection

- Require PR approval
- Require passing CI checks
- No direct pushes to `main`

## Development Process

### Spec-Driven Development

Each feature is defined in a milestone spec before implementation:

```
docs/milestones/phase-0.1-scaffolding.md
```

Specs contain:
- Objective
- Acceptance criteria (behavioural)
- Implementation notes
- Status

### Workflow

1. Read the milestone spec
2. Write failing tests for acceptance criteria
3. Implement until tests pass
4. Refactor
5. Update spec status
6. Open PR

### Using Claude Code

Spec files provide context for Claude Code when planning and implementing features. Reference relevant specs when starting work:

```
# In Claude Code
@docs/ARCHITECTURE.md @docs/milestones/phase-0.1-scaffolding.md

Implement the room graph and basic navigation
```
