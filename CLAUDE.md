# CLAUDE.md - Queatline

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Questline text adventure game engine evolving toward a cooperative MUD platform.

## Repository Overview

This is a monorepo containing multiple projects and folders.

| Path | Purpose |
| `src/Questline/` | the core application code containing the game engine |
| `tests/Questline.Tests/` | the tests for the application code |
| `openspec/` | OpenSpec configuration, specs, and change tracking |
| `content/adventures/` | JSON adventure content |

## `src/Questline/`: Application Code

### Folder Structure
- `Cli/` - The entry point for the terminal - Composition root, a thin client running the game engine
- `Domain/` - Game Rules - Entities, data objects, value objects, domain commands, domain events, command handlers and validators
- `Engine/` - The game engine - Loading content, parsing commands and dispatching commands to handelers
- `Framework/` - Core abstractions and utilities - This is where code for interacting with external systems lives, project agnostic code

## `tests/Questline.Tests/`: Application Test Suite

Follows a similar structure to the main application code project, i.e. tests for componentns of the game engine will live in the `Engine/` folder.

## `openspec/`: OpenSpec

This project uses OpenSpec to follow a Spec-Driven Development methodology. This folder contains configuration, generated `specs/**/SPEC.md` files for features and detailed plans for the feature in the `changes/` folder. Specs are updated via OpenSpec change workflow, not edited directly.

## Development Commands

- `dotnet build` - Builds the solution
- `dotnet test --no-build` - Run all tests
- `dotnet run --project src/Questline` - Run the game

### GitHub Workflows
- **Create a PR**: `gh pr create -a @me -t "<SHORT TILE OF THE CHANGES>"`
- Workflow definitions are located in `.github/workflows/`

## Technical Guidelines

### CSharp
- .Net 10 (LTS)
- Use latest language features

## Additional Resources
- Consult `docs/` for detailed documentation
