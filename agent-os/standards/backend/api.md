# Minimal API Patterns

## Core Principles

1. **Use Minimal APIs** - No controllers, leverage the lightweight endpoint model
2. **Group endpoints by feature** - Each feature has its own endpoint mapping class
3. **No API documentation generation** - No Swagger/OpenAPI in production code
4. **Handlers do the work** - Endpoints are thin wrappers that delegate to handlers

## Endpoint Organization

Each feature has an endpoints class that maps all routes:

```csharp
namespace MyApp.Features.Orders;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapPost("/", CreateOrder);
        group.MapGet("/{id:int}", GetOrder);
        group.MapGet("/", ListOrders);
        group.MapPost("/{id:int}/cancel", CancelOrder);

        return app;
    }

    private static async Task<IResult> CreateOrder(
        CreateOrderRequest request,
        CreateOrderHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(request, ct);

        return result.Match<IResult>(
            success => Results.Created($"/api/orders/{success.OrderId}", success),
            validationFailed => Results.BadRequest(validationFailed.Errors),
            error => Results.Problem(error.Message));
    }

    private static async Task<IResult> GetOrder(
        int id,
        GetOrderHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(id, ct);

        return result.Match<IResult>(
            order => Results.Ok(order),
            notFound => Results.NotFound());
    }

    private static async Task<IResult> ListOrders(
        [AsParameters] ListOrdersQuery query,
        ListOrdersHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(query, ct);

        return Results.Ok(result);
    }

    private static async Task<IResult> CancelOrder(
        int id,
        CancelOrderHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(id, ct);

        return result.Match<IResult>(
            success => Results.NoContent(),
            notFound => Results.NotFound(),
            error => Results.Problem(error.Message));
    }
}
```

## Route Registration

Register all feature endpoints in `Program.cs`:

```csharp
var app = builder.Build();

app.MapOrderEndpoints();
app.MapCustomerEndpoints();
app.MapProductEndpoints();

app.Run();
```

## Request Binding

### Body Parameters
Automatically bound from JSON body:
```csharp
group.MapPost("/", async (CreateOrderRequest request, ...) => ...);
```

### Route Parameters
Use typed parameters with constraints:
```csharp
group.MapGet("/{id:int}", async (int id, ...) => ...);
group.MapGet("/{slug}", async (string slug, ...) => ...);
```

### Query Parameters
Use `[AsParameters]` for complex query objects:
```csharp
public record ListOrdersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Status = null,
    DateTime? FromDate = null);

group.MapGet("/", async ([AsParameters] ListOrdersQuery query, ...) => ...);
```

Or bind individual query parameters:
```csharp
group.MapGet("/", async (int page, int pageSize, string? status, ...) => ...);
```

### Header Parameters
Use `[FromHeader]` attribute:
```csharp
group.MapGet("/", async ([FromHeader(Name = "X-Correlation-Id")] string? correlationId, ...) => ...);
```

## Route Groups

Use groups to apply common configuration:

```csharp
public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
{
    var group = app.MapGroup("/api/orders")
        .WithTags("Orders")
        .RequireAuthorization();

    // Public endpoints
    var publicGroup = app.MapGroup("/api/orders");
    publicGroup.MapGet("/featured", GetFeaturedOrders);

    // Authenticated endpoints
    group.MapPost("/", CreateOrder);
    group.MapGet("/my-orders", GetMyOrders);

    return app;
}
```

## Authentication & Authorization

Apply at group or endpoint level:

```csharp
// Require authentication for all endpoints in group
var group = app.MapGroup("/api/orders")
    .RequireAuthorization();

// Require specific policy
group.MapPost("/", CreateOrder)
    .RequireAuthorization("AdminOnly");

// Allow anonymous for specific endpoint
group.MapGet("/public", GetPublicOrders)
    .AllowAnonymous();
```

## Response Patterns

### Success Responses
```csharp
Results.Ok(data)              // 200 with body
Results.Created(uri, data)    // 201 with location header
Results.NoContent()           // 204 no body
Results.Accepted()            // 202 for async operations
```

### Error Responses
```csharp
Results.BadRequest(errors)    // 400 validation errors
Results.NotFound()            // 404 resource not found
Results.Conflict(message)     // 409 business rule violation
Results.Problem(detail)       // 500 internal error (RFC 7807)
```

### Typed Results
For better compile-time checking:
```csharp
private static async Task<Results<Ok<OrderResponse>, NotFound>> GetOrder(
    int id,
    GetOrderHandler handler,
    CancellationToken ct)
{
    var result = await handler.HandleAsync(id, ct);

    return result.Match<Results<Ok<OrderResponse>, NotFound>>(
        order => TypedResults.Ok(order),
        notFound => TypedResults.NotFound());
}
```

## Guidelines

- **Keep endpoints thin** - No business logic, just request/response handling
- **Use consistent route patterns** - `GET /resources`, `GET /resources/{id}`, `POST /resources`
- **Return appropriate status codes** - Match HTTP semantics
- **Inject handlers, not services** - Let handlers manage their own dependencies
- **Use CancellationToken** - Always pass to async handlers
- **Use TypedResults for complex scenarios** - Better IntelliSense and compile-time safety
