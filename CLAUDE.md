# CLAUDE.md - Questline

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Questline is a parser-driven text adventure engine evolving toward a digital OSR (Old School Renaissance) party-based RPG with optional co-op support. See [`docs/roadmap.md`](docs/roadmap.md) for the current direction and phase plan.

## Repository Overview

This is a monorepo containing multiple projects and folders.

| Path                       | Purpose                                                                     |
|----------------------------|-----------------------------------------------------------------------------|
| `src/Questline/`           | the core application code containing the game engine                        |
| `tests/Questline.Tests/`   | the tests for the application code                                          |
| `devenv/Questline.DevEnv/` | project to orchestrate a local development environment for external systems |
| `content/adventures/`      | JSON adventure content                                                      |
| `docs/`                    | documentation, roadmap, design specs, and project history                   |

## `src/Questline/`: Application Code

### Folder Structure

- `Cli/` - The entry point for the terminal - Composition root, a thin client running the game engine
- `Domain/` - Game Rules - Entities, data objects, value objects and validators
- `Engine/` - The game engine - Loading content, parsing request and sending commands/queries to handlers. Requests,
  responses and request handlers live here.
- `Framework/` - Core abstractions and utilities - This is where code for interacting with external systems lives,
  project agnostic code

### Domain Feature-Folder Convention

`Domain/` is organised by bounded context, a set of capabilities: `Players/`, `Rooms/`, `Shared/`. Each bounded context
folder uses the same internal layout:

| Sub-folder | Contents                          |
|------------|-----------------------------------|
| `Entity/`  | Domain entities and value objects |
| `Data/`    | Data objects (where applicable)   |

New bounded context get their own top-level folder under `Domain/` following this layout. New capabilities related to an
existing bounded context should be added to that context.

## `tests/Questline.Tests/`: Application Test Suite

Follows a similar structure to the main application code project, i.e. tests for componentns of the game engine will
live in the `Engine/` folder.

## `devenv/Questline.DevEnv/`: Development Environment Project

This is an Aspire project, This project is used to orchestrate a local development environment for external systems. It
provides a convenient way to set up and manage dependencies and configurations required for development, such as MongoDB.
Before running the tests or running the game, the development environment must be running. To start the development
environment run the following command:

```
aspire run
```

See @docs/devenv.md for more information.

## `docs/`: Documentation and Design

- `docs/roadmap.md` — project direction and phased plan
- `docs/architecture.md` — architecture overview
- `docs/devenv.md` — local development environment
- `docs/adding_a_new_request.md` — walkthrough for adding a new request
- `docs/superpowers/specs/` — design documents produced via the Superpowers brainstorming workflow
- `docs/history/` — historical project records (e.g. archived OpenSpec changes from earlier workflow)

## Development Commands

- `dotnet build` - Builds the solution
- `dotnet test --no-build` - Run all tests
- `dotnet run --project src/Questline` - Run the game
- `aspire run` - Run the development environment

### GitHub Workflows

- **Create a PR**: `gh pr create -a @me -t "<SHORT TILE OF THE CHANGES>"`
- Workflow definitions are located in `.github/workflows/`

## Technical Guidelines

### C# Conventions

- .Net 10 (LTS)
- Use latest language features
- **File-scoped namespaces** - `namespace Foo.Bar;` (no braces)
- **Records** - Use for requests, responses, and immutable value types
- **Primary constructors** - Use on classes (handlers, builders, attributes) not just records
- **`static abstract` interface members** - `IRequest.CreateRequest` enforces a static factory contract
- **Allman brace style** - Opening Brace on the NEXT line

### Test Conventions

- Tests live under `tests/Questline.Tests/` mirroring source layout.
- The subject under test gets its own folder (e.g. `Engine/Parsers/Parser/`).
- One scenario per file: `When_<scenario>.cs` containing `public class When_<scenario>`.
- Test methods use plain-English fact-based names (`Input_is_parsed_correctly`, `Barrier_blocks_movement_when_locked`).
- xUnit + Shouldly. Tests are written before the production code where possible.

### New Command Checklist

When adding a new game command:

1. Create the request record with `[Verbs("verb", "alias")]` in `Engine/Messages/Requests.cs`.
2. Create the response record with factory methods in `Engine/Messages/Responses.cs`.
3. Create the handler implementing `IRequestHandler<T>` in `Engine/Handlers/`.
4. Register the handler in `Engine/ServiceCollectionExtensions.RegisterCommandHandlers()`.
5. Write tests in `tests/Questline.Tests/Engine/Handlers/<HandlerName>/` using `GameBuilder`/`RoomBuilder`.
6. Update `CHANGELOG.md` and bump `<Version>` in `Directory.Build.props` if shipping a release.

## Additional Resources

- Consult `docs/` for detailed documentation
