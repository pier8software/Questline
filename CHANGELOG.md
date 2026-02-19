# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/),
and this project adheres to [Semantic Versioning](https://semver.org/).

## [Unreleased]

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
