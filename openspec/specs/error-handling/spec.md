# error-handling Specification

## Purpose

Define how the game handles errors, unknown input, and edge cases. All errors are displayed in a consistent, friendly format. Unknown commands produce varied responses and offer typo suggestions.

## Requirements

### Requirement: Graceful error handling

All commands SHALL handle edge cases gracefully. No unhandled exceptions SHALL crash the game.

#### Scenario: Command error

- **WHEN** a command encounters an error (e.g. item not found)
- **THEN** an error result with a descriptive message SHALL be returned

#### Scenario: Unexpected exception

- **WHEN** an unexpected exception occurs during command execution
- **THEN** the game SHALL display a friendly error message and continue running (no crash)

### Requirement: Consistent error format

Errors SHALL be displayed in a consistent, friendly format. Stack traces SHALL only appear in debug mode.

#### Scenario: Error display

- **WHEN** an error result is displayed
- **THEN** the message SHALL be user-friendly (no raw exception text in release mode)

### Requirement: Varied unknown command responses

Unrecognised commands SHALL produce varied, flavourful responses that rotate to maintain personality.

#### Scenario: Unknown command variety

- **WHEN** the player enters multiple unrecognised commands
- **THEN** the responses SHALL vary (not always the same message)

### Requirement: Typo detection with suggestions

When input is close to a valid command (within Levenshtein distance of 2), the game SHALL suggest the correct command with "Did you mean...?"

#### Scenario: Typo suggestion

- **WHEN** the player enters "hlep"
- **THEN** the result SHALL suggest "Did you mean 'help'?"

#### Scenario: No close match

- **WHEN** the player enters "xyzzy" with no close match to any command
- **THEN** no suggestion SHALL be offered (just the standard unknown command response)

## Implementation Notes

### Unknown Command Responses

```csharp
private static readonly string[] UnknownResponses = [
    "I don't understand '{0}'.",
    "That's not something you can do.",
    "'{0}'? I'm not sure what you mean.",
    "You ponder '{0}' but nothing comes of it.",
    "That doesn't make sense here."
];
```

### Levenshtein Distance

Simple implementation for typo detection — compare input against all registered verbs and suggest the closest match within distance 2.

### Error Display

```csharp
// Release: friendly message only
// Debug: include stack trace for diagnostics
```
