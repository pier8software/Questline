## Context

The Questline application already supports a `--mode=deploy-content` run mode that seeds adventure JSON into MongoDB (see `cli-run-modes` spec). The dev environment AppHost (`Questline.DevEnv`) currently defines MongoDB as a raw `AddContainer("mongo", "mongo")` with a hardcoded endpoint — no health checks, no connection string management, no typed resource.

Today, content deployment requires manually running the application against a known-good MongoDB instance. The proposal calls for automating this via a Dockerfile built and orchestrated by Aspire.

## Goals / Non-Goals

**Goals:**

- Automatically deploy game content when the dev environment starts
- Replace the raw mongo container with the typed `AddMongoDB` integration for health checks and connection string support
- Make the MongoDB connection string configurable so Aspire can inject it at runtime
- Keep a single Dockerfile that builds the Questline app, reusable for any run mode

**Non-Goals:**

- Production deployment or CI/CD Docker builds
- Health checks or retry logic inside the content seeder
- Persistent MongoDB volumes (per devenv.md guidance on avoiding persistent containers early)

## Decisions

### 1. Use `AddMongoDB` hosting integration instead of raw `AddContainer`

**Choice**: Replace `AddContainer("mongo", "mongo")` with `AddMongoDB("mongo").AddDatabase("questline")` from `Aspire.Hosting.MongoDB`.

**Why**: The typed integration provides built-in health checks (so `WaitFor` actually verifies MongoDB is accepting connections), automatic connection string generation, and `WithReference` support. The raw container only exposes a running state with no health verification.

**Alternative considered**: Keep `AddContainer` and add a manual health check. Rejected because the integration is purpose-built for this and is a single NuGet reference.

**Package**: `Aspire.Hosting.MongoDB` version `13.1.2` (aligns with AppHost SDK `13.1.1`).

### 2. Use `AddDockerfile` to build the content deployer container

**Choice**: Use Aspire's `AddDockerfile` API to build the Questline app from a `Dockerfile` at the repository root.

**Why**: `AddDockerfile` is built into `Aspire.Hosting` (no extra package), integrates with the dashboard, and supports `WithReference`, `WithArgs`, `WaitFor` — everything needed to wire the container into the resource graph.

**Context path**: `../../` relative to the AppHost project (reaching the repo root where the Dockerfile, solution, and content live).

### 3. Pass `--mode=deploy-content` via `WithArgs`

**Choice**: Use `.WithArgs("--mode=deploy-content")` on the container resource rather than baking the args into the Dockerfile `CMD`.

**Why**: Keeps the Dockerfile generic. The same image could be used for game mode or any future run mode by changing the args. The Dockerfile `ENTRYPOINT` points to the application; args are supplied by Aspire at runtime.

### 4. Wire MongoDB connection string via `WithReference` and configuration

**Choice**: Use `.WithReference(mongo)` to inject the connection string as `ConnectionStrings__questline` environment variable. Update `CliAppBuilder` to read the connection string from `IConfiguration` (falling back to `mongodb://localhost:27017` for backward compatibility).

**Why**: Aspire's `WithReference` sets `ConnectionStrings__{resourceName}` automatically. Using `IConfiguration` with `Microsoft.Extensions.Configuration` (already a dependency) lets the app read from environment variables without any Aspire client packages.

**Alternative considered**: Pass as a custom environment variable and read directly from `Environment.GetEnvironmentVariable`. Rejected because `IConfiguration` already supports env vars and is more idiomatic for .NET apps.

### 5. Use `WaitFor` for MongoDB dependency

**Choice**: `.WaitFor(mongo)` on the content deployer container.

**Why**: With `AddMongoDB`, `WaitFor` blocks until MongoDB's health check reports healthy (accepting connections). This ensures the content seeder doesn't attempt to connect before MongoDB is ready.

### 6. Multi-stage Dockerfile

**Choice**: Standard .NET multi-stage Dockerfile — restore, build, publish, then copy to a runtime image.

**Why**: Keeps the final image small (runtime-only, no SDK). The content JSON file is included via the existing `<Content>` item in the `.csproj` which copies it to the output directory.

## Risks / Trade-offs

**[Risk]** Replacing `AddContainer` with `AddMongoDB` changes port mapping behaviour (Aspire proxies the port).
→ **Mitigation**: The content deployer uses `WithReference` for its connection string, so it gets the correct proxied address. Any other tools connecting directly to `localhost:27017` may need adjustment — but this is a dev environment change and easily reversible.

**[Risk]** Docker build on every `aspire run` adds startup time.
→ **Mitigation**: Docker layer caching means rebuilds are fast when only content changes. The restore and build layers are cached when code hasn't changed.

**[Trade-off]** The Dockerfile lives at the repo root, not in `devenv/`.
→ This is intentional — the Docker build context needs access to the solution file, source code, and content directory. A repo-root Dockerfile is the standard .NET convention.
