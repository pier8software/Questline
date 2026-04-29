## Context

The `context` property in `openspec/config.yaml` is injected into every OpenSpec artifact instruction. It currently contains high-level project identity, tech stack, architecture overview, code style, testing, git workflow, and roadmap information. While useful for orientation, it does not describe the concrete patterns a developer needs to follow when implementing a new feature — the domain folder convention, the request/response/handler pipeline, DI registration, parser discovery, content loading, or test builder APIs.

This change adds that practical implementation guidance so that generated specs, designs, and task lists are grounded in how the codebase actually works.

## Goals / Non-Goals

**Goals:**

- Add a "Domain Structure" section describing the feature-folder convention (Entity / Handlers / Messages / Data)
- Add a "Command Pipeline" section explaining the request → parser → handler → response flow with concrete type names
- Add a "New Feature Checklist" summarising the end-to-end steps to add a command
- Keep the context concise enough that it remains useful (not overwhelming) when injected into artifact prompts

**Non-Goals:**

- Changing the config.yaml schema, rules, or any other property
- Updating CLAUDE.md, docs/, or any application/test code
- Documenting every class — focus on patterns, not exhaustive API reference

## Decisions

### 1. Augment existing context rather than replace it

**Rationale**: The current context sections (Project Identity, Tech Stack, Architecture, etc.) are still valuable for orientation. New sections will be appended after the existing content, keeping the established structure intact.

**Alternative considered**: Replace the entire context with a more implementation-focused version. Rejected because the high-level overview is still useful for specs and proposals that don't need implementation detail.

### 2. Organise new content into three focused sections

The additions will be structured as:
1. **Domain Structure** — folder convention and where new code goes
2. **Command Pipeline Details** — IRequest, @Verbs, IRequestHandler, DI registration, parser discovery
3. **New Feature Checklist** — step-by-step recipe

**Rationale**: These three aspects capture the minimum knowledge needed to go from "I need to add feature X" to a working implementation with tests. They map to the questions most frequently needed: *where* (structure), *how* (pipeline), and *what steps* (checklist).

### 3. Include concrete type names and file paths

The context will reference actual types (`IRequest`, `IRequestHandler<T>`, `GameState`, `GameBuilder`, `RoomBuilder`) and paths (`Domain/<Feature>/Handlers/`, `Engine/ServiceCollectionExtensions.cs`).

**Rationale**: Generic descriptions like "add a handler" are ambiguous. Concrete names eliminate guesswork and produce more accurate artifacts.

**Alternative considered**: Keep it abstract to avoid coupling to current code. Rejected because the context already references concrete types (e.g., `System.Text.Json`) and the value comes from specificity.

## Risks / Trade-offs

- **[Staleness]** → The context may drift from the code as the project evolves. Mitigated by keeping descriptions pattern-focused rather than exhaustive, and by reviewing config.yaml when architecture changes.
- **[Context length]** → Adding content increases token usage in every artifact prompt. Mitigated by keeping additions concise (bullet points, not prose) and avoiding full code examples — short inline snippets only where essential.
