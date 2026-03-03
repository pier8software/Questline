## MODIFIED Requirements

### Requirement: Game mode runs the interactive game loop

The game mode SHALL configure all game services and start the interactive game loop. Adventure content SHALL already be present in the database, deployed via `deploy-content` mode before the game is started.

#### Scenario: Game mode starts the game

- **WHEN** the application runs in game mode
- **THEN** the application SHALL start the interactive game loop
- **AND** the player SHALL be able to interact with the game via console input

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
