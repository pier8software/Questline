# Secrets Management

## Overview

Use AWS Secrets Manager everywhere - LocalStack locally, real AWS in production.

## Storing Secrets

### Create secrets via CLI

```bash
# Production
aws secretsmanager create-secret \
  --name questline/api-keys \
  --secret-string '{"stripe":"sk_live_xxx","sendgrid":"SG.xxx"}'

# Local (LocalStack)
awslocal secretsmanager create-secret \
  --name questline/api-keys \
  --secret-string '{"stripe":"sk_test_xxx","sendgrid":"SG.test"}'
```

### LocalStack Initialization

Add to `localstack/init/init-aws.sh`:

```bash
awslocal secretsmanager create-secret \
  --name questline/api-keys \
  --secret-string '{"stripe":"sk_test_xxx","sendgrid":"SG.test"}'
```

## Fetching Secrets in Code

### Secrets Configuration Class

```csharp
public class ApiKeySecrets
{
    public string Stripe { get; init; } = "";
    public string Sendgrid { get; init; } = "";
}
```

### Fetch at Startup

```csharp
public static class SecretsExtensions
{
    public static async Task<T> GetSecretAsync<T>(
        this IAmazonSecretsManager client,
        string secretName)
    {
        var response = await client.GetSecretValueAsync(
            new GetSecretValueRequest { SecretId = secretName });

        return JsonSerializer.Deserialize<T>(response.SecretString)
            ?? throw new InvalidOperationException($"Failed to deserialize secret {secretName}");
    }
}

// Program.cs
var secretsManager = builder.Services.BuildServiceProvider()
    .GetRequiredService<IAmazonSecretsManager>();

var apiKeys = await secretsManager.GetSecretAsync<ApiKeySecrets>("questline/api-keys");
builder.Services.AddSingleton(apiKeys);
```

### AWS SDK Registration

```csharp
// Works with both LocalStack (via ServiceURL) and real AWS
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSecretsManager>();
```

## Secret Naming Convention

```
{app-name}/{secret-category}

Examples:
questline/api-keys      # Third-party API keys
questline/db-creds      # Database credentials
questline/jwt-signing   # JWT signing keys
```

## ECS Integration

Reference secrets in task definition:

```json
{
  "secrets": [
    {
      "name": "STRIPE_API_KEY",
      "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789:secret:questline/api-keys:stripe::"
    }
  ]
}
```

## Guidelines

- **Never commit secrets** - Not in code, not in appsettings, not in .env files
- **Use consistent paths** - Same secret names in LocalStack and AWS
- **Fetch once at startup** - Cache secrets, don't fetch per-request
- **Rotate secrets** - Use Secrets Manager rotation for production
- **Least privilege** - ECS task role only accesses needed secrets
