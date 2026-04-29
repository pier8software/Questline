## 1. Draft New Context Sections

- [x] 1.1 Add "Domain Structure" section to `context` in `openspec/config.yaml` describing the feature-folder convention (`Domain/<Feature>/Entity|Handlers|Messages|Data`), what each sub-folder contains, and how the `Shared/` feature is used for cross-cutting concerns
- [x] 1.2 Add "Command Pipeline Details" section covering: `IRequest` with `static abstract CreateRequest(string[] args)`, `@Verbs` attribute for parser discovery, `IRequestHandler<T>` with `Handle(GameState, T)` signature, `RequestSender` dispatch, and the single-line DI registration in `Engine/ServiceCollectionExtensions.RegisterCommandHandlers()`
- [x] 1.3 Add "New Feature Checklist" section with a concise step-by-step recipe: (1) create request record with `@Verbs`, (2) create response record with factory methods, (3) create handler implementing `IRequestHandler<T>`, (4) register handler in `RegisterCommandHandlers()`, (5) write tests using `GameBuilder`/`RoomBuilder`

## 2. Verify

- [x] 2.1 Run `openspec status` to confirm config.yaml is still valid and parseable after edits
- [x] 2.2 Review final context length is concise — aim for no more than ~120 lines total (existing + new)
