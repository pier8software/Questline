## Why

Game content must be seeded into MongoDB before the game can run. Today this requires manually invoking `--mode=deploy-content` against a running Mongo instance. Adding a Dockerfile allows the Aspire dev environment to automatically deploy content as a container that depends on the MongoDB container being ready — removing the manual step and ensuring content is always seeded when the dev environment starts.

## What Changes

- Add a `Dockerfile` at the repository root that builds the Questline application and sets `--mode=deploy-content` as the default entrypoint argument
- Update `AppHost.cs` in `Questline.DevEnv` to build and run the content deployer as a container, configured with `--mode=deploy-content` args and a dependency on the MongoDB container being ready
- Wire the MongoDB connection string from the Aspire resource into the content deployer container

## Non-goals

- Changing the existing `deploy-content` run mode logic or `ContentDeploymentApp` behaviour
- Creating Dockerfiles for the game mode or any other run mode
- Adding health checks or retry logic to the content seeder itself
- Deploying content to non-local environments

## Capabilities

### New Capabilities

- `content-deploy-container`: Defines the Dockerfile and Aspire AppHost integration for running content deployment as a container in the dev environment

### Modified Capabilities

- `cli-run-modes`: The deploy-content mode must accept a MongoDB connection string via configuration/environment variable rather than only the hardcoded `mongodb://localhost:27017` value, so the Aspire-managed connection string can be passed through

## Impact

- **New file**: `Dockerfile` at repository root
- **Modified**: `devenv/Questline.DevEnv/AppHost.cs` — new container resource with dependency on mongo
- **Modified**: `src/Questline/Cli/CliAppBuilder.cs` — MongoDB connection string must be configurable (currently hardcoded)
- **Dependencies**: No new NuGet packages; uses existing Aspire container hosting APIs
