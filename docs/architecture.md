# Architecture

Questline uses Domain Driven Design (DDD) principles to ensure a clean separation of concerns.

The CLI is the entry point to the game and is a thin layer on top of the engine.
The Engine is responsible for parsing input into commands and dispatching them to handlers.
The Framework provides infrastructure services to load content and persist game state.
The Domain defines the game rules and entities. The handlers are responsible for executing commands against the domain
entities and returning results to the engine.
The engine will then pass the result back to the cli for rendering.

## Domain Organisation

`Domain/` is organised by feature, each with a consistent internal structure:

```
Domain/
├── Players/
│   ├── Entity/       # Player
│   ├── Handlers/     # MovePlayerCommandHandler, DropItemCommandHandler, ...
│   └── Messages/     # Requests.cs, Responses.cs
├── Rooms/
│   ├── Entity/       # Room, Exit, Direction
│   ├── Data/         # RoomData
│   ├── Handlers/     # GetRoomDetailsHandler, TakeItemHandler
│   └── Messages/     # Requests.cs, Responses.cs
└── Shared/
    ├── Entity/       # Item, Inventory
    ├── Data/         # GameState, AdventureData, ItemData
    ├── Handlers/     # QuitGameHandler
    └── Messages/     # Requests.cs, Responses.cs
```

Each feature's `Messages/Requests.cs` contains request records annotated with `[Verbs("...")]` and implementing `IRequest`.
Each feature's `Messages/Responses.cs` contains response records implementing `IResponse`, which format their own `Message` property.

## Game Pipeline

```
Cli Input → Parser → IRequest → RequestSender → IRequestHandler<T> → IResponse → Renderer
```

- **Parser** tokenises input, looks up the first token (verb) in a dictionary built at startup by scanning all `IRequest` types for `[Verbs]` attributes via reflection. It delegates remaining tokens to the matching type's `static CreateRequest(string[] args)` factory, returning a `ParseResult`.
- **RequestSender** receives `(GameState, IRequest)`, closes the open generic `IRequestHandler<>` over the request's runtime type, resolves the concrete handler from DI, and invokes `Handle` via reflection.
- **Handlers** receive `(GameState, TRequest)`, execute domain logic, and return an `IResponse`. Handlers never write to console directly.
- **GameEngine** ties it together: calls `Parser.Parse`, then `RequestSender.Send`, and returns the `IResponse` to the CLI for rendering.

## Dependency Flow

As a thin layer on top of the engine, the CLI depends on the engine.
The engine depends on the domain, for game rules and framework for persistence and content loading.

```
Cli → Engine → Framework
        ↓
        Domain
```
