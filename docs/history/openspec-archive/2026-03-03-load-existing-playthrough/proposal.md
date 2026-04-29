## Why

Players currently lose all progress when they quit because there is no way to resume a saved playthrough. The save infrastructure already persists game state to MongoDB on every meaningful action, but the game flow goes straight from login to "new adventure" with no option to load an existing one. Introducing a start menu with a "Load Game" option completes the save-and-load spec's restoration flow and makes the persistence layer useful to players.

## What Changes

- Introduce a **start menu phase** after login that presents "New Game" and "Load Game" options
- "New Game" proceeds through the existing select-adventure and character-creation flow
- "Load Game" queries the player's saved playthroughs and presents them as a selectable list
- Selecting a saved playthrough restores game state (character, inventory, location, world state) and resumes the game loop from where the player left off
- The existing `GamePhase` state machine gains a new `StartMenu` phase between `Login` and `NewAdventure`
- A new repository query lists all playthroughs belonging to a player

## Non-goals

- Deleting or managing saved playthroughs (future work)
- Multiple save slots per playthrough (auto-save on every action is the current model)
- Manual save/load commands during gameplay (already covered by the save-and-load spec)
- Player account creation or authentication (login is a simple username prompt)
- Cloud sync or cross-device save transfer

## Capabilities

### New Capabilities

- `start-menu`: The start menu phase that presents "New Game" and "Load Game" options after login, routing the player to the appropriate flow

### Modified Capabilities

- `game-loop`: The game loop phase sequence changes — a start menu phase is inserted between login and new adventure, and loading a playthrough bypasses character creation to resume directly into gameplay
- `save-and-load`: The load/restore side of save-and-load is implemented — querying saved playthroughs by player, selecting one, and restoring full game state to resume play

## Impact

- **Engine**: `GameEngine` gains a `StartMenu` phase and logic to list/load playthroughs
- **Persistence**: `PlaythroughRepository` needs a `FindByPlayer` query method
- **Domain**: `Playthrough` or `GameSession` may need to associate playthroughs with a player identity
- **Cli**: `GameApp` and `ResponseFormatter` need to handle the new start menu responses
- **Tests**: New handler and integration tests for the start menu flow and playthrough loading
