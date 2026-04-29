## Why

The `character-creation` spec introduces a `Character` model that represents the in-game avatar, separate from the `Player` (the human). Currently, `Player` owns `Location` and `Inventory` directly, but these are properties of the character being played, not the player themselves. Moving them to `Character` aligns the domain model with the Player/Character separation defined in the spec and prepares the codebase for future features like party play where a player may switch characters.

## What Changes

- **Move `Location` from `Player` to `Character`**: The character's current room is an attribute of the avatar, not the human controlling it.
- **Move `Inventory` from `Player` to `Character`**: Items are carried by the character, not the player.
- **Update `Player` to access `Location` and `Inventory` through `Character`**: All existing code that reads/writes `Player.Location` or `Player.Inventory` will route through `Player.Character`.
- **BREAKING**: `Player.Location` and `Player.Inventory` are no longer direct properties — consumers must go through `Player.Character`.

## Capabilities

### New Capabilities

_(none — this is a refactor of existing behaviour)_

### Modified Capabilities

- `character-creation`: Update the `Player` and `Character` model definitions to reflect that `Location` and `Inventory` belong to `Character`, not `Player`.

## Impact

- `Domain/Players/Entity/Player.cs` — remove `Location` and `Inventory`, add `Character` property
- `Domain/Characters/Entity/` — new `Character.cs` with `Location`, `Inventory`, and character-creation fields (Name, Race, Class, Level, Experience, Stats)
- `Engine/Handlers/` — all handlers referencing `Player.Location` or `Player.Inventory` must update to `Player.Character.Location` / `Player.Character.Inventory`
- `Domain/Shared/Entity/GameState.cs` — may need updates if it references Player properties directly
- `tests/` — test setup and assertions referencing `Player.Location` / `Player.Inventory` must update

## Non-goals

- Combat, XP, or levelling mechanics
- Race/class-specific behaviour or stat modifiers
- Multiplayer or party-play networking
- Changing the command pipeline or parser
