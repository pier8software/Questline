# Base Game Loop — Implementation Plan

## Summary

Build a classic text adventure game loop following the REPL pattern (Read-Evaluate-Print Loop) like Zork and Colossal Cave Adventure. Player enters commands, game updates state, displays result.

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                      Game Loop                          │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌─────────┐ │
│  │  Read    │→ │  Parse   │→ │ Execute  │→ │  Print  │ │
│  │ (input)  │  │ (command)│  │ (update) │  │ (output)│ │
│  └──────────┘  └──────────┘  └──────────┘  └─────────┘ │
└─────────────────────────────────────────────────────────┘
         │              │              │
         ▼              ▼              ▼
    Console.In    CommandParser    GameState
```

## Core Components

1. **GameState** — Current room, inventory, object states, flags
2. **CommandParser** — Parses "go north" → `Command { Verb: "go", Noun: "north" }`
3. **CommandHandler** — Executes commands, returns result text
4. **GameLoop** — REPL orchestrator

## File Structure

```
src/Questline/
├── Program.cs           # Game loop entry point
├── GameState.cs         # Game state record
├── Command.cs           # Command record
├── CommandParser.cs     # Input parsing
├── CommandHandler.cs    # Command execution
└── Questline.csproj

tests/Questline.Tests/
├── CommandParserTests.cs
├── CommandHandlerTests.cs
└── Questline.Tests.csproj
```

## Verification

1. **Build:** `dotnet build`
2. **Run:** `dotnet run --project src/Questline`
3. **Test:** `dotnet test`
4. **Manual test:**
   - Start game, see welcome message
   - Type "look" — see room description
   - Type "go north" — change rooms (or get "can't go that way")
   - Type "inventory" — see empty inventory
   - Type "quit" — exit game
