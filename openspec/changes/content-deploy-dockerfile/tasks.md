## 1. Configurable MongoDB Connection String

- [x] 1.1 Write tests for `CliAppBuilder` reading the connection string from `IConfiguration`, verifying it uses `ConnectionStrings:questline` when present and falls back to `mongodb://localhost:27017` when absent
- [x] 1.2 Update `CliAppBuilder` to accept `IConfiguration` and read the MongoDB connection string from `ConnectionStrings:questline` with a default fallback of `mongodb://localhost:27017`
- [x] 1.3 Update `Program.cs` to build an `IConfiguration` from environment variables and pass it to `CliAppBuilder`

## 2. Dockerfile

- [x] 2.1 Create a multi-stage `Dockerfile` at the repository root: restore dependencies, build, publish, then copy to a .NET runtime image with `ENTRYPOINT ["dotnet", "Questline.dll"]`
- [x] 2.2 Verify the Dockerfile builds successfully with `docker build` and that the output image contains `the-goblins-lair.json`

## 3. AppHost Integration

- [x] 3.1 Add `Aspire.Hosting.MongoDB` package (version `13.1.2`) to `Questline.DevEnv.csproj`
- [x] 3.2 Replace `AddContainer("mongo", "mongo")` with `AddMongoDB("mongo").AddDatabase("questline")` in `AppHost.cs`
- [x] 3.3 Add `AddDockerfile("content-deployer", "../../")` resource to `AppHost.cs` with `.WithArgs("--mode=deploy-content")`, `.WithReference(mongo)`, and `.WaitFor(mongo)`
- [x] 3.4 Run `aspire run` and verify the content deployer container starts after MongoDB is healthy, seeds content, and exits successfully
