## Why

Phase 0.4 of the roadmap adds puzzle mechanics to the game engine. Players need to interact with barriers that block exits, use items to unlock them, and examine items and room features for clues. The `Exit` model already carries an optional `BarrierId` (from the refactor-exit-model change), and the adventure JSON already references `"barrier": "iron-door"` — but nothing consumes this data yet.

## What Changes

- **Barrier entity** (`Domain/Rooms/Entity/Barrier.cs`): Lockable barrier with unlock-item reference, blocked/unlock messages, and mutable `IsUnlocked` state
- **Feature entity** (`Domain/Rooms/Entity/Feature.cs`): Examinable room object with keyword matching
- **GameState**: Extended with a barrier dictionary and `GetBarrier(string? id)` lookup
- **Room**: Extended with a `Features` list
- **Movement**: `MovePlayerCommandHandler` checks barriers before allowing movement; returns `BlockedMessage` when locked
- **Look/Move display**: `RoomDetailsResponse` and `PlayerMovedResponse` include locked barrier descriptions in room output
- **Use command**: `use <item> [on <target>]` — targeted or contextual barrier unlocking
- **Examine command**: `examine <target>` — searches inventory items, room items, then room features by name/keywords
- **Content**: Adventure JSON gains top-level `barriers` array and per-room `features` arrays; `GameContentLoader` builds both
- **Data layer**: `BarrierData`, `FeatureData` DTOs for JSON deserialisation; `AdventureData.Barriers` property; `RoomData.Features` property

## Non-goals

- Multiple barriers per exit
- Barrier types beyond item-unlock (combination locks, riddles, timed barriers)
- Consuming items on use (the key stays in inventory)
- NPCs or dialogue-based puzzles
- Combat or enemies

## Capabilities

### Modified Capabilities

- `puzzles-and-barriers`: All requirements now implemented — use command, barriers, examine, room features, content loading
- `world-model`: Room gains Features list; GameState gains barrier storage
- `content-loading`: Adventure JSON extended with barriers and features arrays
