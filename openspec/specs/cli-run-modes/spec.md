# cli-run-modes Specification

## Purpose

Define the CLI run modes that control the application's execution path. The `--mode` argument selects between interactive game play and operational tasks like content deployment, ensuring each mode registers only the services it needs.

## Requirements

### Requirement: Application supports run modes via --mode argument

The application SHALL accept a `--mode` command-line argument that determines the execution path. When no `--mode` argument is provided, the application SHALL default to game mode.

#### Scenario: No arguments defaults to game mode

- **WHEN** the application is started with no arguments
- **THEN** the application SHALL run in game mode

#### Scenario: Explicit game mode

- **WHEN** the application is started with `--mode=game`
- **THEN** the application SHALL run in game mode

#### Scenario: Deploy-content mode

- **WHEN** the application is started with `--mode=deploy-content`
- **THEN** the application SHALL run in deploy-content mode

#### Scenario: Invalid mode value

- **WHEN** the application is started with `--mode=invalid`
- **THEN** the application SHALL display an error message listing the valid modes
- **AND** the application SHALL exit with a non-zero exit code

### Requirement: Game mode runs the interactive game loop

The game mode SHALL configure all game services and start the interactive game loop. Adventure content SHALL already be present in the database, deployed via `deploy-content` mode before the game is started.

#### Scenario: Game mode starts the game

- **WHEN** the application runs in game mode
- **THEN** the application SHALL start the interactive game loop
- **AND** the player SHALL be able to interact with the game via console input

### Requirement: Deploy-content mode seeds content and exits

The deploy-content mode SHALL load adventure JSON files, deploy them to the database, and exit without starting the interactive game loop.

#### Scenario: Deploy-content seeds to database

- **WHEN** the application runs in deploy-content mode
- **THEN** the application SHALL load adventure content from JSON files
- **AND** the application SHALL store the content in the database
- **AND** the application SHALL exit with a zero exit code

#### Scenario: Deploy-content produces no interactive I/O

- **WHEN** the application runs in deploy-content mode
- **THEN** the application SHALL NOT prompt for user input
- **AND** the application SHALL NOT start the game loop

#### Scenario: Deploy-content reports completion

- **WHEN** the application runs in deploy-content mode and content is deployed successfully
- **THEN** the application SHALL write a confirmation message to the console

### Requirement: Each run mode registers only its required services

The application SHALL register only the services needed for the selected run mode. Game mode SHALL register the game engine, parser, command handlers, and persistence services. Game mode SHALL NOT register content seeding or JSON file loading services. Deploy-content mode SHALL register only persistence and content seeding services. The MongoDB connection string SHALL be read from `IConfiguration` rather than hardcoded, allowing it to be provided via environment variables, command-line arguments, or configuration files.

#### Scenario: Game mode does not register content seeding services

- **WHEN** the application runs in game mode
- **THEN** `IContentSeeder` and `JsonFileLoader` SHALL NOT be registered in the service container

#### Scenario: Deploy-content mode does not register game engine

- **WHEN** the application runs in deploy-content mode
- **THEN** the game engine, parser, and command handlers SHALL NOT be registered in the service container

#### Scenario: MongoDB connection string read from configuration

- **WHEN** the application starts in any run mode
- **THEN** the MongoDB connection string SHALL be read from the `ConnectionStrings:questline` configuration key

#### Scenario: Default connection string when configuration is absent

- **WHEN** the application starts and no `ConnectionStrings:questline` configuration value is present
- **THEN** the application SHALL use `mongodb://localhost:27017` as the default connection string

#### Scenario: Connection string provided via environment variable

- **WHEN** the `ConnectionStrings__questline` environment variable is set
- **THEN** the application SHALL use that value as the MongoDB connection string
