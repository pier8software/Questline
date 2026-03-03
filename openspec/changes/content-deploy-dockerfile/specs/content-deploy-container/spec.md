## ADDED Requirements

### Requirement: Dockerfile builds the Questline application

A `Dockerfile` at the repository root SHALL build the Questline application using a multi-stage build. The restore and build stages SHALL use the .NET SDK image. The final stage SHALL use the .NET runtime image. The built image SHALL include the adventure content JSON files in the output directory.

#### Scenario: Multi-stage build produces runtime image

- **WHEN** the Dockerfile is built
- **THEN** the final image SHALL contain the published Questline application
- **AND** the final image SHALL use the .NET runtime base image (not the SDK)

#### Scenario: Adventure content included in image

- **WHEN** the Dockerfile is built
- **THEN** the `the-goblins-lair.json` content file SHALL be present in the application's output directory

#### Scenario: Entrypoint is the Questline application

- **WHEN** a container is created from the built image with no arguments
- **THEN** the container SHALL execute the Questline application

### Requirement: AppHost defines content deployer as a Dockerfile resource

The Aspire AppHost SHALL define a container resource built from the repository-root Dockerfile using `AddDockerfile`. The resource SHALL be configured with `--mode=deploy-content` arguments so the container runs the content deployment run mode.

#### Scenario: Content deployer is built from Dockerfile

- **WHEN** the Aspire AppHost starts
- **THEN** the content deployer resource SHALL be built from the Dockerfile at the repository root

#### Scenario: Content deployer runs in deploy-content mode

- **WHEN** the content deployer container starts
- **THEN** the container SHALL receive `--mode=deploy-content` as arguments

### Requirement: Content deployer depends on MongoDB being ready

The content deployer container SHALL wait for the MongoDB resource to be healthy before starting. This ensures the database is accepting connections before the seeder attempts to write.

#### Scenario: Content deployer waits for MongoDB

- **WHEN** the Aspire AppHost starts
- **THEN** the content deployer container SHALL NOT start until the MongoDB resource reports healthy

#### Scenario: Content deployer starts after MongoDB is ready

- **WHEN** the MongoDB resource reports healthy
- **THEN** the content deployer container SHALL start and seed content into the database

### Requirement: Content deployer receives MongoDB connection string from Aspire

The content deployer container SHALL receive the MongoDB connection string via Aspire's `WithReference` mechanism. The connection string SHALL be injected as the `ConnectionStrings__questline` environment variable.

#### Scenario: Connection string injected via environment

- **WHEN** the content deployer container starts
- **THEN** the `ConnectionStrings__questline` environment variable SHALL contain the MongoDB connection string for the `questline` database

### Requirement: AppHost uses typed MongoDB hosting integration

The AppHost SHALL use the `AddMongoDB` hosting integration (from `Aspire.Hosting.MongoDB`) instead of a raw container definition. The integration SHALL define a named database resource that provides connection string and health check support.

#### Scenario: MongoDB resource uses hosting integration

- **WHEN** the Aspire AppHost is configured
- **THEN** MongoDB SHALL be defined using `AddMongoDB` with an `AddDatabase("questline")` child resource

#### Scenario: MongoDB resource has health checks

- **WHEN** the MongoDB resource is defined via `AddMongoDB`
- **THEN** the resource SHALL have built-in health checks that verify MongoDB is accepting connections
