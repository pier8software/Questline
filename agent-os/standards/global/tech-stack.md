# Tech Stack

## Target Framework
- **.NET 10** - Latest LTS release
- **C# 13** - Latest language version with all modern features enabled

## Application Types
- **Blazor** - Interactive web UI (Server, WebAssembly, or Auto render modes)
- **Worker Services** - Background processing and hosted services
- **Web APIs** - RESTful APIs using Minimal API patterns

## Core Libraries

### Data Access
- **AWS SDK for .NET** - DynamoDB via Document Model
  - Repository per entity with Load, Save, Query methods
  - No ORM - direct document operations with explicit mapping
- **Amazon SQS** - Message queuing for async operations

### Results & Error Handling
- **OneOf** - Discriminated unions for result types
  - Replace exceptions with explicit error types
  - Make failure states part of the type system

### Validation
- **FluentValidation** - Declarative validation rules
  - Validators co-located with features
  - Clear, readable validation logic

### Logging & Observability
- **Serilog** - Structured logging
  - Semantic logging with message templates
  - Enrichers for contextual information
- **OpenTelemetry** - Distributed tracing and metrics
  - Traces, metrics, and logs correlation
  - Vendor-agnostic observability

### Testing
- **xUnit** - Test framework
  - Modern, extensible test runner
- **Shouldly** - Assertion library
  - Readable assertion syntax
  - Better error messages

## Package References

```xml
<!-- Core -->
<PackageReference Include="AWSSDK.DynamoDBv2" Version="3.*" />
<PackageReference Include="AWSSDK.SQS" Version="3.*" />
<PackageReference Include="AWSSDK.SecretsManager" Version="3.*" />
<PackageReference Include="AWSSDK.Extensions.NETCore.Setup" Version="3.*" />
<PackageReference Include="OneOf" Version="3.*" />
<PackageReference Include="FluentValidation" Version="11.*" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.*" />
<PackageReference Include="Ulid" Version="1.*" />

<!-- Logging & Observability -->
<PackageReference Include="Serilog.AspNetCore" Version="8.*" />
<PackageReference Include="Serilog.Sinks.Console" Version="6.*" />
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.*" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.*" />
<PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.*" />

<!-- Testing -->
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
<PackageReference Include="xunit" Version="2.*" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
<PackageReference Include="Shouldly" Version="4.*" />
```

## Dependency Injection
- **Manual registration** - No auto-discovery or assembly scanning
- Register services explicitly in `Program.cs` or extension methods
- Keep registrations close to the features they serve

## Architecture
- **Vertical Slice Architecture** - Features organized by capability, not layer
- **CQRS-style** - Separate Commands and Queries within features
- **No Mediator** - Direct handler invocation, no MediatR
