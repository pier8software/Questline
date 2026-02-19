## Why

The game currently creates a Player with only an Id, Location, and Inventory — there is no concept of a Character with a name, race, class, or stats. Phase 0.5 introduces character creation so the player can name their avatar and receive a default Human Fighter with rolled ability scores, laying the groundwork for future progression mechanics.

## What Changes

- Add `Character` record with Name, Race, Class, Level, Experience, and `CharacterStats` (ability scores + health)
- Add `Race` and `CharacterClass` enums to the domain
- Add dice-rolling abstraction for stat generation (3d6 per ability, 1d8 for starting health)
- Add character name validation (2-24 chars, alphanumeric + spaces, no leading/trailing whitespace)
- Add `Player.Character` property linking Player to their Character
- Add `stats` command that displays character name, race, class, level, and stat values
- Modify game start flow: prompt for character name before entering the game loop
- Modify welcome message to include the character's name

## Non-goals

- Race/class selection (all characters are Human Fighter for now)
- Stat modifiers by race or class
- Combat, XP, or levelling
- Persistent character saves (covered by save-and-load spec)

## Capabilities

### New Capabilities

- `character-creation`: Character model (name, race, class, stats), name validation, dice rolling, stats command, and game start flow changes

### Modified Capabilities

- `game-loop`: Game start flow changes to prompt for character name and display a personalised welcome message before entering the command loop

## Impact

- `Domain/Players/Entity/Player.cs` — gains a `Character` property
- `Domain/Characters/Entity/` — new `Character` record, `Race` enum, `CharacterClass` enum, `CharacterStats` record
- `Domain/Characters/` — name validation logic
- `Engine/Messages/` — new `StatsQuery` request and `StatsResponse` response
- `Engine/Handlers/` — new `StatsQueryHandler`
- `Cli/CliApp.cs` — modified startup flow to prompt for name and display welcome message
- `Framework/` or `Domain/` — dice-rolling abstraction (`IDice` or similar)
