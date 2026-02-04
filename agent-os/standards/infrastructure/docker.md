# Docker & Local Development

## .NET Aspire for Local Orchestration

Use Aspire AppHost to orchestrate services locally with built-in dashboard for logs, traces, and metrics.

### AppHost Project

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var localstack = builder.AddContainer("localstack", "localstack/localstack")
    .WithEnvironment("SERVICES", "dynamodb,sqs")
    .WithEndpoint(port: 4566, targetPort: 4566, name: "gateway")
    .WithHealthCheck("gateway", path: "/_localstack/health");

var api = builder.AddProject<Projects.MyApp_Api>("api")
    .WithEnvironment("AWS__ServiceURL", localstack.GetEndpoint("gateway"))
    .WithEnvironment("AWS__Region", "us-east-1")
    .WaitFor(localstack);

builder.Build().Run();
```

### Project Structure

```
src/
├── MyApp.AppHost/           # Aspire orchestration
│   └── Program.cs
├── MyApp.ServiceDefaults/   # Shared config (OpenTelemetry, health checks)
│   └── Extensions.cs
└── MyApp.Api/               # Your application
    └── Program.cs
```

### ServiceDefaults

Configure OpenTelemetry and health checks once, share across projects:

```csharp
public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
{
    builder.ConfigureOpenTelemetry();
    builder.AddDefaultHealthChecks();
    return builder;
}
```

## Multi-Stage Dockerfile

For production deployments:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.sln .
COPY */*.csproj ./
RUN for file in *.csproj; do mkdir -p ${file%.*} && mv $file ${file%.*}/; done
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MyApp.Api.dll"]
```

## LocalStack Initialization

Create `localstack/init/init-aws.sh` for table/queue setup:

```bash
#!/bin/bash
awslocal dynamodb create-table \
  --table-name Orders \
  --attribute-definitions AttributeName=PK,AttributeType=S AttributeName=SK,AttributeType=S \
  --key-schema AttributeName=PK,KeyType=HASH AttributeName=SK,KeyType=RANGE \
  --billing-mode PAY_PER_REQUEST

awslocal sqs create-queue --queue-name order-events
```

## Guidelines

- **Use Aspire for local dev** - Single command starts everything with observability
- **Use Dockerfiles for deployment** - Multi-stage builds for production images
- **Pin image versions in production** - Never use `latest` in deployed environments
- **Use `.dockerignore`** - Exclude bin/, obj/, .git/, etc.
