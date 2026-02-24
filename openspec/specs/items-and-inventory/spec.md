# items-and-inventory Specification

## Purpose

Define items as world entities and inventory as a reusable container. Players can pick up, carry, and drop items. Rooms display their items. This lays the groundwork for puzzle mechanics and gear.

## Requirements

### Requirement: Item entity with identity and description

An Item SHALL have a unique `Id`, a `Name`, and a `Description`.

#### Scenario: Item properties

- **WHEN** an Item is created with Id "lamp", Name "brass lamp", Description "A shiny brass lamp."
- **THEN** the Item SHALL expose those values

### Requirement: Rooms have items

A Room SHALL have an `Items` property (`ImmutableList<Item>`) and a `FindItemByName(string name)` method for case-insensitive lookup. WorldBuilder SHALL support adding items to rooms.

#### Scenario: Room with items

- **WHEN** a Room is built with a "brass lamp" item via WorldBuilder
- **THEN** the Room's Items SHALL contain "brass lamp"

### Requirement: Player has inventory

The Character SHALL have an `Inventory` property (`ImmutableList<Item>`) that starts empty, and a `FindInventoryItemByName(string name)` method for case-insensitive lookup.

#### Scenario: New player inventory

- **WHEN** a Player is created
- **THEN** the Player's Inventory SHALL be empty

### Requirement: Get command transfers item from room to player

The `get <item>` command SHALL remove the named item from the current room and add it to the player's inventory.

#### Scenario: Pick up existing item

- **WHEN** the player executes `get brass lamp` and "brass lamp" is in the room
- **THEN** the item SHALL be in the player's inventory and removed from the room

#### Scenario: Item not in room

- **WHEN** the player executes `get sword` and no "sword" is in the room
- **THEN** an error result SHALL be returned

#### Scenario: No argument

- **WHEN** the player executes `get` with no item name
- **THEN** an error result SHALL be returned

#### Scenario: Take alias

- **WHEN** the player executes `take lamp`
- **THEN** it SHALL behave the same as `get lamp`

#### Scenario: Case-insensitive matching

- **WHEN** the player executes `get BRASS LAMP` and the room contains "brass lamp"
- **THEN** the item SHALL be picked up successfully

### Requirement: Drop command transfers item from player to room

The `drop <item>` command SHALL remove the named item from the player's inventory and place it in the current room.

#### Scenario: Drop carried item

- **WHEN** the player executes `drop brass lamp` and has "brass lamp" in inventory
- **THEN** the item SHALL be in the room and removed from inventory

#### Scenario: Item not in inventory

- **WHEN** the player executes `drop sword` and has no "sword" in inventory
- **THEN** an error result SHALL be returned

### Requirement: Inventory command lists carried items

The `inventory` command SHALL list all items the player is carrying, or indicate when carrying nothing. Aliases: `inv`, `i`.

#### Scenario: Carrying items

- **WHEN** the player carries "brass lamp" and "rusty key" and executes `inventory`
- **THEN** the result SHALL list both item names

#### Scenario: Empty inventory

- **WHEN** the player carries nothing and executes `inventory`
- **THEN** the result SHALL indicate "not carrying anything"

### Requirement: Look and go include room items

The `look` and `go` results SHALL include items visible in the room. Items are hidden from output when none are present.

#### Scenario: Look with items

- **WHEN** the player looks in a room containing "brass lamp"
- **THEN** the result SHALL contain "You can see" and "brass lamp"

#### Scenario: Look without items

- **WHEN** the player looks in a room with no items
- **THEN** the result SHALL NOT contain "You can see"

### Requirement: Multi-word item names

The command pipeline SHALL support item names consisting of multiple words (e.g. "brass lamp").

#### Scenario: Multi-word get

- **WHEN** the player executes `get brass lamp`
- **THEN** the item name "brass lamp" SHALL be matched as a single name
