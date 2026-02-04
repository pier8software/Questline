# Shaping Notes: Terminal.Gui Refactor

## Scope

**In Scope:**
- Replace Console REPL with Terminal.Gui TUI
- Create GameWindow with output and input panels
- Preserve all existing game logic (CommandParser, CommandHandler, GameState)
- Maintain testability of game logic layer

**Out of Scope:**
- New game features or content
- Changes to command parsing logic
- Changes to game state management
- Adding new commands

## Key Decisions

### 1. Separation of Concerns
Keep UI layer completely separate from game logic. The Cli folder contains all testable game logic; the new Tui folder contains only UI code.

### 2. Terminal.Gui v2
Using version 2.x of Terminal.Gui for modern API and better layout system.

### 3. Window-based Architecture
GameWindow extends Window (not Toplevel) for future flexibility in adding multiple windows/dialogs.

### 4. State Management
GameState remains immutable and managed within GameWindow. Each command execution returns a new state.

## Context

The original REPL loop was a quick prototype. Moving to Terminal.Gui provides:
- Better visual presentation
- Scrollable output history
- Proper terminal handling
- Foundation for future UI enhancements (menus, dialogs, etc.)
