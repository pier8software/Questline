# Environment Configuration

## Configuration Precedence

From lowest to highest priority:

1. `appsettings.json` — Base defaults
2. `appsettings.{Environment}.json` — Environment overrides
3. Environment variables — Runtime overrides (production)

## appsettings Structure

### appsettings.json (Base)

```json
{
  "AWS": {
    "Region": "us-east-1"
  },
  "Features": {
    "NewCheckoutFlow": false,
    "BetaFeatures": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### appsettings.Development.json (Local)

```json
{
  "AWS": {
    "ServiceURL": "http://localhost:4566"
  },
  "Features": {
    "BetaFeatures": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

### Production (via environment variables)

```bash
# ECS task definition or container config
AWS__Region=us-east-1
Features__NewCheckoutFlow=true
```

## Strongly-Typed Configuration

Bind configuration to classes:

```csharp
public class AwsOptions
{
    public const string SectionName = "AWS";
    public string Region { get; init; } = "us-east-1";
    public string? ServiceURL { get; init; }  // Only set for LocalStack
}

public class FeatureOptions
{
    public const string SectionName = "Features";
    public bool NewCheckoutFlow { get; init; }
    public bool BetaFeatures { get; init; }
}
```

### Registration

```csharp
builder.Services.Configure<AwsOptions>(
    builder.Configuration.GetSection(AwsOptions.SectionName));

builder.Services.Configure<FeatureOptions>(
    builder.Configuration.GetSection(FeatureOptions.SectionName));
```

### Usage

```csharp
public class OrderHandler(IOptions<FeatureOptions> features)
{
    public async Task HandleAsync(...)
    {
        if (features.Value.NewCheckoutFlow)
        {
            // New logic
        }
    }
}
```

## Environment Detection

Use `ASPNETCORE_ENVIRONMENT` or `DOTNET_ENVIRONMENT`:

- `Development` — Local with Aspire/LocalStack
- `Production` — Deployed to AWS

```csharp
if (builder.Environment.IsDevelopment())
{
    // LocalStack-specific setup
}
```

## Guidelines

- **Never commit secrets** - Use secrets management for sensitive values
- **Use strongly-typed options** - Avoid `IConfiguration` direct access
- **Keep appsettings.json minimal** - Only non-sensitive defaults
- **Document env vars** - List required variables in README
- **Use `:` or `__` for nesting** - `AWS__Region` or `AWS:Region`
