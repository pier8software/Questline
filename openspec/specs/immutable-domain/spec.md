### Requirement: Domain entities are immutable after construction
All domain entities (`Character`, `Room`, `Barrier`, `Item`) and the `Inventory` value object SHALL be immutable after construction. No public mutable properties (setters) or mutable collections SHALL be exposed.

#### Scenario: Entity properties cannot be reassigned after creation
- **WHEN** a domain entity is constructed
- **THEN** all properties SHALL be read-only (no public setters)

#### Scenario: Entity collections cannot be modified externally
- **WHEN** a domain entity exposes a collection property (e.g., `Room.Items`, `Room.Exits`)
- **THEN** the collection SHALL be an immutable type that cannot be modified by callers

### Requirement: Character state changes return new instances
The `Character` entity SHALL provide methods for state changes that return a new `Character` instance with the modified state, leaving the original instance unchanged.

#### Scenario: Character moves to a new location
- **WHEN** `MoveTo(locationId)` is called on a `Character`
- **THEN** a new `Character` instance SHALL be returned with `Location` set to the new location
- **THEN** the original `Character` instance SHALL retain its original `Location`

#### Scenario: Character picks up an item
- **WHEN** `AddInventoryItem(item)` is called on a `Character`
- **THEN** a new `Character` instance SHALL be returned with the item added to its `Inventory`
- **THEN** the original `Character` instance's `Inventory` SHALL NOT contain the new item

#### Scenario: Character drops an item
- **WHEN** `RemoveInventoryItem(item)` is called on a `Character`
- **THEN** a new `Character` instance SHALL be returned with the item removed from its `Inventory`
- **THEN** the original `Character` instance's `Inventory` SHALL still contain the item

### Requirement: Room state changes return new instances
The `Room` entity SHALL provide methods for state changes that return a new `Room` instance with the modified state, leaving the original instance unchanged.

#### Scenario: Item added to a room
- **WHEN** `AddItem(item)` is called on a `Room`
- **THEN** a new `Room` instance SHALL be returned with the item present in `Items`
- **THEN** the original `Room` instance's `Items` SHALL NOT contain the new item

#### Scenario: Item removed from a room
- **WHEN** `RemoveItem(item)` is called on a `Room`
- **THEN** a new `Room` instance SHALL be returned with the item absent from `Items`
- **THEN** the original `Room` instance's `Items` SHALL still contain the item

### Requirement: Barrier state changes return new instances
The `Barrier` entity SHALL provide an `Unlock()` method that returns a new `Barrier` instance with `IsUnlocked` set to `true`, leaving the original instance unchanged.

#### Scenario: Barrier is unlocked
- **WHEN** `Unlock()` is called on a locked `Barrier`
- **THEN** a new `Barrier` instance SHALL be returned with `IsUnlocked` equal to `true`
- **THEN** the original `Barrier` instance SHALL retain `IsUnlocked` equal to `false`

### Requirement: Inventory operations return new instances
The `Inventory` value object SHALL provide `Add` and `Remove` methods that return new `Inventory` instances, leaving the original unchanged.

#### Scenario: Item added to inventory
- **WHEN** `Add(item)` is called on an `Inventory`
- **THEN** a new `Inventory` instance SHALL be returned containing the item
- **THEN** the original `Inventory` instance SHALL NOT contain the new item

#### Scenario: Item removed from inventory
- **WHEN** `Remove(item)` is called on an `Inventory`
- **THEN** a new `Inventory` instance SHALL be returned without the item
- **THEN** the original `Inventory` instance SHALL still contain the item

### Requirement: GameState provides entity replacement methods
`GameState` SHALL provide methods to replace domain entities by identity, allowing handlers to swap in new immutable instances after state changes.

#### Scenario: Player is updated in game state
- **WHEN** `UpdatePlayer(player)` is called on `GameState`
- **THEN** subsequent access to `GameState.Player` SHALL return the new player instance

#### Scenario: Room is updated in game state
- **WHEN** `UpdateRoom(room)` is called on `GameState` with a room whose `Id` matches an existing room
- **THEN** subsequent calls to `GetRoom(id)` SHALL return the new room instance

#### Scenario: Barrier is updated in game state
- **WHEN** `UpdateBarrier(barrier)` is called on `GameState` with a barrier whose `Id` matches an existing barrier
- **THEN** subsequent calls to `GetBarrier(id)` SHALL return the new barrier instance

### Requirement: Handlers use immutable entity APIs
All command handlers that modify domain state SHALL use the entity's immutable mutation methods and update `GameState` via its replacement methods. Handlers SHALL NOT directly mutate entity properties or collections.

#### Scenario: Taking an item uses immutable APIs
- **WHEN** a player takes an item from a room
- **THEN** the handler SHALL call `Room.RemoveItem` and `Character.AddInventoryItem` to produce new instances
- **THEN** the handler SHALL update `GameState` with the new `Room` and `Player` instances

#### Scenario: Dropping an item uses immutable APIs
- **WHEN** a player drops an item into a room
- **THEN** the handler SHALL call `Character.RemoveInventoryItem` and `Room.AddItem` to produce new instances
- **THEN** the handler SHALL update `GameState` with the new `Room` and `Player` instances

#### Scenario: Moving a player uses immutable APIs
- **WHEN** a player moves to another room
- **THEN** the handler SHALL call `Character.MoveTo` to produce a new `Character` instance
- **THEN** the handler SHALL update `GameState` with the new `Player` instance

#### Scenario: Using an item on a barrier uses immutable APIs
- **WHEN** a player uses an item to unlock a barrier
- **THEN** the handler SHALL call `Barrier.Unlock` to produce a new `Barrier` instance
- **THEN** the handler SHALL update `GameState` with the new `Barrier` instance
