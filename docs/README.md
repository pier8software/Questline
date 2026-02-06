# Questline Documentation

This folder contains the specification documents for the Questline project.

## Contents

| Document | Purpose |
|----------|---------|
| [PRODUCT_PLAN.md](PRODUCT_PLAN.md) | Mission, goals, and roadmap |
| [ARCHITECTURE.md](ARCHITECTURE.md) | System design and component structure |
| [CODE_STANDARDS.md](CODE_STANDARDS.md) | Tech stack, style, testing, and git workflow |

## Milestones

Phase 0 milestone specifications are in `milestones/`:

| Milestone | Focus |
|-----------|-------|
| [phase-0.1-scaffolding.md](milestones/phase-0.1-scaffolding.md) | Project structure, world model, navigation |
| [phase-0.2-items-inventory.md](milestones/phase-0.2-items-inventory.md) | Items, get/drop, inventory |
| [phase-0.3-content-loading.md](milestones/phase-0.3-content-loading.md) | JSON content files, 5-room dungeon |
| [phase-0.4-puzzles.md](milestones/phase-0.4-puzzles.md) | Use command, barriers, examine |
| [phase-0.5-character-creation.md](milestones/phase-0.5-character-creation.md) | Character model, name entry |
| [phase-0.6-save-load.md](milestones/phase-0.6-save-load.md) | Persistence, save/load |
| [phase-0.7-polish.md](milestones/phase-0.7-polish.md) | Help, error handling, victory |

## Using with Claude Code

Reference relevant specs when working on features:

```
@docs/ARCHITECTURE.md @docs/milestones/phase-0.1-scaffolding.md

Implement the room graph and basic navigation
```

## Status Tracking

Each milestone spec includes a **Status** field:

- **Not Started** — Work has not begun
- **In Progress** — Active development
- **Complete** — All acceptance criteria met
