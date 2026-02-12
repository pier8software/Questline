# Architecture

Questline uses Domain Driven Design (DDD) principles to ensure a clean separation of concerns.

The CLI is the entry point to the game and is a thin layer on top of the engine.
The Engine is responsible for parsing input into commands and dispatching them to handlers.
The Framework provides infrastructure services to load content and persist game state.
The Domain defines the game rules and entities. The handlers are responsible for executing commands against the domain
entities and returning results to the engine.
The engine will then pass the result back to the cli for rendering.

## Game Pipeline

```
Cli Input → Parser → Command → Handler → Result → Renderer
```

- Parser tokenises input into `ParsedInput` (verb + arguments)
- Dispatcher routes to registered handler by verb
- Handlers receive command objects, return `CommandResult` variants
- Handlers never write to console directly

## Dependency Flow

As a thin layer on top of the engine, the CLI depends on the engine.
The engine depends on the domain, for game rules and framework for persistence and content loading.

```
Cli → Engine → Framework
        ↓
        Domain
```
