## MODIFIED Requirements

### Requirement: Use command applies item in context

The `use <item>` command SHALL attempt to use an item from the player's inventory in the current context. The `use <item> on <target>` form SHALL use the item on a specific target.

#### Scenario: Use on already unlocked barrier

- **WHEN** the player executes `use rusty key on iron door` and the iron door is already unlocked
- **THEN** an informative message SHALL be returned indicating the barrier is already unlocked

#### Scenario: Target not found in room

- **WHEN** the player executes `use key on iron door` but no barrier named "iron door" exists in the current room
- **THEN** an error message SHALL be returned

### Requirement: Barriers block exits

A barrier SHALL prevent movement through an exit until unlocked.

#### Scenario: Look omits barrier when unlocked

- **WHEN** the player looks in a room where a barrier has been unlocked
- **THEN** the look result SHALL NOT include a barrier description line for that exit

### Requirement: Examine command shows detailed descriptions

The `examine <item>` command SHALL show the detailed description of an item in inventory or in the current room. The `examine <feature>` command SHALL show detailed descriptions of room features matched by name or keywords. Aliases: `inspect`.

#### Scenario: Examine room item

- **WHEN** the player examines "torch" which is in the current room (not inventory)
- **THEN** the result SHALL contain the item's detailed description

#### Scenario: Examine feature by keyword

- **WHEN** the player examines "symbols" in a room with a "strange symbols" feature that has "symbols" in its keywords
- **THEN** the result SHALL contain the feature's detailed description

#### Scenario: Examine unknown target

- **WHEN** the player examines a target that is not in inventory, room items, or room features
- **THEN** an error message SHALL be returned

## REMOVED Sections

### Implementation Notes

_(Spec is now implemented — code is the source of truth per project conventions)_
