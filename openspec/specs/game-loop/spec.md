# game-loop Specification

## Purpose

Define the game's main loop: startup display, prompt cycle, result rendering, and graceful termination. The game loop orchestrates the interaction between the player and the command pipeline.

## Requirements

### Requirement: Game displays initial room on start

The game SHALL load adventure content and save the initial world state to the repository, then prompt for character creation, save the character to the game profile, and finally display a welcome message including the character's name followed by the starting room's name, description, available items, locked barrier descriptions, and exits.

#### Scenario: Game startup with character creation

- **WHEN** the game starts and the player enters character name "Thorin"
- **THEN** adventure content SHALL be loaded and saved to the repository before character creation
- **AND** the character SHALL be saved to the game profile after creation completes
- **AND** the output SHALL contain a welcome message including "Thorin"
- **AND** the output SHALL contain the starting room name, description, and available exits

#### Scenario: Game startup displays items in starting room

- **WHEN** the game starts and the starting room contains items
- **THEN** the output SHALL display a "You can see:" line listing the item names

#### Scenario: Game startup displays locked barriers

- **WHEN** the game starts and the starting room has exits blocked by locked barriers
- **THEN** the output SHALL display each locked barrier's description

### Requirement: Game prompts for input in a loop

The game SHALL repeatedly display a prompt and accept player input until termination.

#### Scenario: Prompt loop

- **WHEN** the game is running
- **THEN** the game SHALL display a prompt character and wait for input after each command completes

### Requirement: Game displays command results

The game SHALL display the message from each CommandResult returned by the command pipeline.

#### Scenario: Successful command

- **WHEN** a command returns a result with Message "You pick up the brass lamp."
- **THEN** the game SHALL display "You pick up the brass lamp."

### Requirement: Quit command exits gracefully

The `quit` command SHALL end the game loop gracefully.

#### Scenario: Quit returns QuitResult

- **WHEN** QuitCommandHandler.Execute is called with any valid GameState and QuitCommand
- **THEN** the result SHALL be of type `QuitResult`

#### Scenario: QuitResult indicates success

- **WHEN** QuitCommandHandler returns a QuitResult
- **THEN** the result's `Success` property SHALL be `true`

#### Scenario: QuitResult contains farewell message

- **WHEN** QuitCommandHandler returns a QuitResult
- **THEN** the result's `Message` property SHALL be `"Goodbye!"`

#### Scenario: Game loop terminates on QuitResult

- **WHEN** the command pipeline returns a QuitResult
- **THEN** the game loop SHALL stop and the farewell message SHALL be displayed

### Requirement: EOF terminates gracefully

The game SHALL exit gracefully when input reaches end-of-file (e.g. piped input or Ctrl+D), including during character setup.

#### Scenario: EOF on input during game loop

- **WHEN** the input stream reaches EOF during the game loop
- **THEN** the game SHALL terminate without error

#### Scenario: EOF on input during character setup

- **WHEN** the input stream reaches EOF during character setup (before a character is created)
- **THEN** the game SHALL terminate without error and without entering the game loop
