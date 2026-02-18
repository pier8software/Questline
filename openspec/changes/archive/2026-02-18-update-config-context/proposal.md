## Why

The `context` property in `openspec/config.yaml` describes the project at a high level but lacks the practical implementation patterns needed to build new features. When working through OpenSpec changes, the context is injected into artifact instructions — but currently it doesn't explain *how* to add a command, wire up a handler, or follow the project's conventions. Adding implementation-focused guidance will produce higher-quality specs, designs, and task lists.

## What Changes

- Expand the `context` property in `openspec/config.yaml` with:
  - **Domain feature-folder convention** — the Entity / Handlers / Messages / Data sub-folder layout and where new code goes
  - **Request/Response pattern** — how to create `IRequest` records with `@Verbs`, `IResponse` records with factory methods, and `IRequestHandler<T>` implementations
  - **Registration & discovery** — the single DI registration line in `ServiceCollectionExtensions.RegisterCommandHandlers()` and the parser's reflection-based verb discovery
  - **Content loading pipeline** — JSON adventure file format, `GameContentLoader`, flexible exit serialisation
  - **Test conventions** — builder fluent API (`GameBuilder`, `RoomBuilder`), `FakeConsole`, namespace conventions, and naming patterns
  - **End-to-end feature checklist** — a concise step-by-step recipe for adding a new command from request to test

## Capabilities

### New Capabilities

_(none — this change does not introduce new game capabilities)_

### Modified Capabilities

_(none — no spec-level behaviour changes)_

## Impact

- **File changed**: `openspec/config.yaml` (context property only)
- **No code changes** — this is a process/tooling improvement
- **All future OpenSpec artifacts** will benefit from richer context when generating specs, designs, and tasks

## Non-goals

- Rewriting or restructuring the config.yaml schema or rules
- Updating CLAUDE.md or any other documentation files
- Changing any application or test code
