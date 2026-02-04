# Terminal.Gui Refactoring Plan

## Summary

Refactor Questline's terminal interface from Console.ReadLine/WriteLine REPL to Terminal.Gui TUI while preserving existing game logic and testability.

## Spec Folder

`agent-os/specs/2026-02-04-1200-terminal-gui-refactor/`

---

## Tasks

### Task 0: Create Feature Worktree

**Per git-workflow standard:** All new features MUST be developed using git worktrees.

```bash
# From main worktree, create new branch and worktree
git worktree add ../.worktrees/feature-terminal-gui -b feature/terminal-gui

# Change to new worktree
cd ../.worktrees/feature-terminal-gui
```

All subsequent tasks are performed in the feature worktree, NOT on main.

### Task 1: Save Spec Documentation

Create `agent-os/specs/2026-02-04-1200-terminal-gui-refactor/` with:
- **plan.md** — This full plan
- **shape.md** — Shaping notes (scope, decisions, context)
- **standards.md** — Coding style and testing standards
- **references.md** — Terminal.Gui documentation references

### Task 2: Add Terminal.Gui Package

**File:** `src/Questline/Questline.csproj`

Add package reference:
```xml
<ItemGroup>
  <PackageReference Include="Terminal.Gui" Version="2.*" />
</ItemGroup>
```

### Task 3: Create GameWindow Class

**File:** `src/Questline/Tui/GameWindow.cs` (NEW)

Create a `GameWindow` class that:
- Extends Terminal.Gui `Window`
- Contains output panel (scrollable `TextView` in `FrameView`)
- Contains input panel (`TextField` in `FrameView`)
- Handles Enter key to submit commands
- Integrates with existing `CommandParser` and `CommandHandler`
- Manages `GameState` internally

**Layout:**
```
╭───────────── Questline (Ctrl+Q to quit) ─────────────╮
│ ╭─────────────── Adventure ─────────────────────────╮ │
│ │ [scrollable game output / room descriptions]      │ │
│ │                                                   │ │
│ ╰───────────────────────────────────────────────────╯ │
│ ╭─────────────── Command ───────────────────────────╮ │
│ │ [text input field]                                │ │
│ ╰───────────────────────────────────────────────────╯ │
╰─────────────────────────────────────────────────────╯
```

### Task 4: Update Program.cs Entry Point

**File:** `src/Questline/Program.cs`

Replace REPL loop with Terminal.Gui initialization:
```csharp
Application.Init();
try
{
    var parser = new CommandParser();
    var handler = new CommandHandler();
    var gameWindow = new GameWindow(parser, handler);
    Application.Run(gameWindow);
    gameWindow.Dispose();
}
finally
{
    Application.Shutdown();
}
```

### Task 5: Verify Tests Pass

Run existing tests to ensure game logic remains intact:
- `tests/Questline.Tests/Cli/CommandParserTests.cs`
- `tests/Questline.Tests/Cli/CommandHandlerTests.cs`

---

## Files to Modify

| File | Action |
|------|--------|
| `src/Questline/Questline.csproj` | Add Terminal.Gui package |
| `src/Questline/Program.cs` | Replace REPL with Terminal.Gui init |
| `src/Questline/Tui/GameWindow.cs` | Create new TUI window class |

## Files Unchanged (Reuse As-Is)

| File | Purpose |
|------|---------|
| `src/Questline/Cli/Command.cs` | Command data model |
| `src/Questline/Cli/CommandParser.cs` | Input parsing (UI-agnostic) |
| `src/Questline/Cli/CommandHandler.cs` | Game logic (UI-agnostic) |
| `src/Questline/Cli/CommandResult.cs` | Result model |
| `src/Questline/Cli/GameState.cs` | State management |

---

## Verification

1. **Build:** `dotnet build src/Questline`
2. **Run tests:** `dotnet test tests/Questline.Tests`
3. **Manual test:** `dotnet run --project src/Questline`
   - Verify TUI layout displays correctly
   - Type commands (look, go north, help, quit)
   - Verify output scrolls and command input clears after submission
   - Verify Ctrl+Q exits the application

---

## Standards Applied

- **global/git-workflow** — Use worktrees for feature development, not main branch
- **global/coding-style** — File-scoped namespaces, primary constructors, modern C# features
- **testing/test-writing** — Existing tests remain valid; game logic stays testable
