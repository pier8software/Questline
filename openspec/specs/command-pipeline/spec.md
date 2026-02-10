# command-pipeline Specification

## Purpose

Define the command processing pipeline that transforms raw player input into game actions. The pipeline comprises a parser (tokenisation), command factories (mapping tokens to command objects), a dispatcher (routing commands to handlers), and handlers (executing game logic and returning results).

## Requirements

### Requirement: Parser tokenises input into verb and arguments

The parser SHALL split raw input into a verb and an array of argument strings.

#### Scenario: Input with verb and arguments

- **WHEN** the parser receives "go north"
- **THEN** the verb SHALL be "go" and arguments SHALL be ["north"]

#### Scenario: Input with verb only

- **WHEN** the parser receives "look"
- **THEN** the verb SHALL be "look" and arguments SHALL be empty

#### Scenario: Input with multiple arguments

- **WHEN** the parser receives "get brass lamp"
- **THEN** the verb SHALL be "get" and arguments SHALL be ["brass", "lamp"]

### Requirement: Parser is case-insensitive

The parser SHALL treat input case-insensitively when matching verbs.

#### Scenario: Uppercase verb

- **WHEN** the parser receives "LOOK"
- **THEN** it SHALL match the "look" verb handler

### Requirement: Parser ignores extra whitespace

The parser SHALL trim leading/trailing whitespace and collapse multiple spaces between tokens.

#### Scenario: Padded input

- **WHEN** the parser receives "  go   north  "
- **THEN** the verb SHALL be "go" and arguments SHALL be ["north"]

### Requirement: Dispatcher routes commands to registered handlers

The dispatcher SHALL route a parsed verb to its registered handler.

#### Scenario: Registered verb

- **WHEN** "look" is registered with a LookCommandHandler
- **THEN** dispatching verb "look" SHALL invoke LookCommandHandler

#### Scenario: Unregistered verb

- **WHEN** "fly" is not registered with any handler
- **THEN** dispatching verb "fly" SHALL return an error result

### Requirement: Handlers registered with verb and aliases

A handler SHALL be registered with a primary verb and zero or more aliases.

#### Scenario: Alias routing

- **WHEN** GetCommandHandler is registered with verbs ["get", "take"]
- **THEN** both "get lamp" and "take lamp" SHALL route to GetCommandHandler

### Requirement: Command factories create typed command objects

Each handler registration SHALL include a factory function that converts argument arrays into typed command objects.

#### Scenario: Factory creates command from arguments

- **WHEN** the "go" factory receives arguments ["north"]
- **THEN** it SHALL produce a GoCommand with the parsed direction

#### Scenario: Factory returns null for invalid arguments

- **WHEN** a factory cannot create a valid command from the given arguments
- **THEN** the dispatcher SHALL return an error result

### Requirement: Handlers return results, not console output

Handlers SHALL return a CommandResult object and SHALL NOT write directly to the console.

#### Scenario: Handler returns result

- **WHEN** a handler executes successfully
- **THEN** it SHALL return a CommandResult with Success=true and a Message
