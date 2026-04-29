# Questline Roadmap

## Project Direction

Questline is a parser-driven text adventure engine evolving toward a **digital OSR (Old School Renaissance) party-based RPG** in the spirit of Old School Essentials and Dungeon Crawl Classics.

The original direction was a cooperative MUD platform. The current direction narrows that to:

- A solo player controls a fixed party of four player characters through a story-driven dungeon.
- Optional co-op, where friends share a party and own subsets of the characters (Phase 6).
- Levelling and class progression follow the DCC funnel pattern: start as level-0 commoners, survivors emerge from a funnel adventure as level-1 classed characters.

This roadmap supersedes the earlier "MUD platform" framing and the Phase 0.7 polish list. The Phase 0.7 polish items (help system, error handling, victory detection, command shortcuts) are subsumed into later phases as needed.

## Phase 0 — Original Foundation (complete)

- [x] 0.1 Scaffolding: world model, room graph, look/go commands, game loop, quit
- [x] 0.2 Items & Inventory: Item entity, inventory, get/drop/inventory commands
- [x] 0.3 Content Loading: JSON adventure files, loader, validation, 5-room dungeon
- [x] 0.4 Puzzles: use command, barriers, examine, room features
- [x] 0.5 Character Creation: name entry, default Human Fighter, stats command
- [x] 0.6 Save/Load: persist and restore game state via MongoDB

## Pivot Phases

### Phase 1 — Foundation pivot *(in design)*

Replace single-character `Playthrough` with a `Party` of four level-0 characters.

- `Party` as a first-class domain entity.
- Action attribution: parser distinguishes party-level commands from per-PC commands.
- Live stats: HP mutates, ability score modifiers are queryable.
- Generic dice/check primitive (`d20 + mod vs DC`) with an `IDice` abstraction.
- Turn counter ticking once per accepted command.
- Funnel-style party creation: roll four random level-0 characters with random race, ability scores, and HP.

*No combat, spells, equipment, or class selection yet.*

### Phase 2 — Combat core

Encounter entity, initiative, attack and AC rolls, damage, saves applied to PCs, monster entities, parser additions for `attack`, `cast`, and per-PC combat actions. Death becomes real; the funnel becomes a real funnel.

### Phase 3 — Equipment & dungeon procedure

Item state (open/closed, lit/extinguished, worn/wielded), equipment slots, containers, encumbrance, light sources burning per turn, dungeon turn clock. DCC occupation tables and starter equipment land here.

### Phase 4 — Class features & Vancian magic

Spell prep and cast, thief skills, cleric turning, warrior abilities. Class selection on funnel survival (level 0 → 1 promotion). **Pick OSE or DCC as the concrete reference here** — earlier phases are mostly system-agnostic.

### Phase 5 — Adventure scripting & random tables

Event/rule system for room triggers and story moments. Generic predicate-based gating (replaces `Barrier`). Random table abstraction. Wandering monsters, reaction rolls, level-up flow.

### Phase 6 — Co-op

Multi-client session. Character ownership per connected human. Co-op-specific UX.

## Cross-cutting threads

- **XP and levelling** thread through Phases 2–5 (XP awards in 2, level-up flow in 5).
- **Parser sophistication** (prepositions, pronouns, conjunctions, disambiguation) progresses as each phase needs it.
- **Story scripting** is intentionally light — these are dungeon-crawl modules, not Pillars-of-Eternity narratives.
