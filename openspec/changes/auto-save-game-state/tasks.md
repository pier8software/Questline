## 1. Setup

- [x] 1.1 Add `MongoDB.Driver` NuGet package to the `Questline` project
- [x] 1.2 Create `Framework/Persistence/` folder

## 2. Repository Interface & Document Model

- [x] 2.1 Create `IGameStateRepository` interface in `Framework/Persistence/` with a `Save(GameState)` method
- [x] 2.2 Create `GameStateDocument` DTO and embedded document types in `Framework/Persistence/` mapping the full game state structure (player, character, rooms, barriers, adventure metadata, version)

## 3. AutoSave Decorator

- [x] 3.1 Write tests for `AutoSaveDecorator<TRequest>`: calls inner handler, then calls `Save` on the repository, returns the inner handler's response
- [x] 3.2 Implement `AutoSaveDecorator<TRequest>` in `Framework/Mediator/` wrapping `IRequestHandler<TRequest>` and `IGameStateRepository`

## 4. MongoDB Repository Implementation

- [x] 4.1 Write integration tests for `MongoGameStateRepository`: first save inserts a document, subsequent saves update the existing document, saved document contains full game state
- [x] 4.2 Implement `MongoGameStateRepository` in `Framework/Persistence/` with upsert semantics and domain-to-document mapping

## 5. DI Registration

- [x] 5.1 Create `RegisterHandler<TRequest, THandler>` helper method in `ServiceCollectionExtensions` that registers the concrete handler wrapped with `AutoSaveDecorator`
- [x] 5.2 Refactor existing handler registrations to use the new helper method
- [x] 5.3 Register `IGameStateRepository` as `MongoGameStateRepository` and configure the MongoDB client

## 6. Startup Flow Refactor

- [x] 6.1 Write tests for the new startup sequence: content loads and saves before character creation, character saves to profile after creation, starting room still displays correctly
- [x] 6.2 Refactor `GameEngine` to separate content loading from game initialisation so the world state can be saved before character creation
- [x] 6.3 Refactor `CliApp.Run()` to follow the new order: load content + save profile → character creation + save character → start game loop

## 7. Finalise

- [x] 7.1 Verify all existing tests pass with the decorator in place (mock `IGameStateRepository` in unit tests)
- [x] 7.2 Update `CHANGELOG.md` and bump version in `Directory.Build.props`
