# Coding Standards: Terminal.Gui Refactor

## C# Style

- File-scoped namespaces (`namespace Foo;`)
- Primary constructors where appropriate
- Modern C# features (records, pattern matching, etc.)
- Nullable reference types enabled

## File Organization

```
src/Questline/
├── Cli/                    # Game logic (UI-agnostic)
│   ├── Command.cs
│   ├── CommandHandler.cs
│   ├── CommandParser.cs
│   ├── CommandResult.cs
│   └── GameState.cs
├── Tui/                    # Terminal.Gui UI
│   └── GameWindow.cs
└── Program.cs              # Entry point
```

## Testing

- Game logic in Cli/ remains unit-testable
- UI code in Tui/ is not unit-tested (integration/manual testing)
- Existing tests must continue to pass

## Terminal.Gui Conventions

- Use FrameView for bordered sections
- Use Fill layout for responsive sizing
- Clean up resources in Dispose
- Handle Ctrl+Q for quit shortcut
