# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/),
and this project adheres to [Semantic Versioning](https://semver.org/).

## [Unreleased]

## [0.6.0] - 2026-03-03

### Added

- MongoDB persistence for rooms, playthroughs, and adventures replacing in-memory storage
- Persistence framework with `Document`, `DomainEntity`, `Repository`, and `IDataContext` abstractions
- MongoDB collection naming conventions (PascalCase, CamelCase, SnakeCase, KebabCase)
- Local development environment using Aspire with MongoDB container orchestration
- CLI run modes via `--mode` flag (`game` default, `deploy-content` for seeding)
- Content deployment mode to seed adventure content and exit without entering the game loop
- Dockerfile for building and running the content deployer container
- `RunMode` enum and `IRunMode` interface for modular execution paths
- Application flow diagram in docs

### Changed

- Game engine methods and all handlers converted to async (`async Task`)
- Game engine interface refactored: replaced `AdventureContent` and `IGameContentLoader` with `IRoomRepository` and `IPlaythroughRepository`
- `GameState` moved from `Domain.Shared` to `Engine.Core`; handlers use `Adventure` for room and barrier access
- Domain refactored from immutable records to mutable classes with direct in-place updates
- `Inventory` class removed in favour of `List<Item>` across the domain
- Character properties moved from `Player` to `Character` model
- MongoDB connection string made configurable via `IConfiguration` with localhost fallback

## [0.5.0] - 2026-02-19

### Added

- Character creation flow: game prompts for a character name on startup
- Character model with name, race, class, level, experience, and stats
- `Race` and `CharacterClass` enums for future progression
- Ability score generation using dice rolls (3d6 per score)
- Starting health rolled with 1d8 (max health fixed at 8 for Fighter)
- Character name validation (2-24 characters, alphanumeric and spaces, no leading/trailing whitespace)
- `stats` command to display character name, race, class, level, and ability scores
- Personalised welcome message using character name on game start
- `IDice` abstraction for testable dice rolling
- `WorldContent` record to decouple content loading from player construction

### Changed

- Player now owns a Character (separate Player and Character models)
- Game start flow prompts for character name before entering the command loop
- `GameContentLoader` returns `WorldContent` instead of `GameState`; player is created at runtime after name entry

## [0.4.0] - 2026-02-18

### Added

- Puzzle and barrier system: locked exits that require items to unlock
- `use` command to use inventory items on barriers (e.g. `use rusty key on iron door`)
- `examine` / `inspect` command to inspect items and room features
- Room features with keyword-based examination
- Locked barrier descriptions shown in room details
- `version` / `ver` command to display the current application version
- Semantic versioning via `Directory.Build.props`
- This changelog
