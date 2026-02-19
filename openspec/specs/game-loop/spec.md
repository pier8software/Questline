# game-loop Specification

## Purpose

Define the game's main loop: startup display, prompt cycle, result rendering, and graceful termination. The game loop orchestrates the interaction between the player and the command pipeline.

## Requirements

### Requirement: Game displays initial room on start

The game SHALL prompt for a character name, display a welcome message including the character's name, and then display the starting room's name, description, and exits when the game begins.

#### Scenario: Game startup with character creation

- **WHEN** the game starts and the player enters character name "Thorin"
- **THEN** the output SHALL contain a welcome message including "Thorin"
- **AND** the output SHALL contain the starting room name, description, and available exits

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

The game SHALL exit gracefully when input reaches end-of-file (e.g. piped input or Ctrl+D).

#### Scenario: EOF on input

- **WHEN** the input stream reaches EOF
- **THEN** the game SHALL terminate without error
