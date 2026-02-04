# Vertical Slice Architecture

## Core Principles

1. **Organize by feature, not by layer** - Each feature contains everything it needs
2. **Minimize coupling between features** - Features are independent units
3. **No mediator pattern** - Direct handler invocation keeps code simple and traceable
4. **CQRS-style separation** - Commands modify state, Queries read state

## Folder Structure

```
src/
└── MyApp/
    ├── Features/
    │   ├── Orders/
    │   │   ├── Commands/
    │   │   │   ├── CreateOrder/
    │   │   │   │   ├── CreateOrderHandler.cs
    │   │   │   │   ├── CreateOrderRequest.cs
    │   │   │   │   ├── CreateOrderResponse.cs
    │   │   │   │   └── CreateOrderValidator.cs
    │   │   │   └── CancelOrder/
    │   │   │       ├── CancelOrderHandler.cs
    │   │   │       └── CancelOrderRequest.cs
    │   │   ├── Queries/
    │   │   │   ├── GetOrder/
    │   │   │   │   ├── GetOrderHandler.cs
    │   │   │   │   └── GetOrderResponse.cs
    │   │   │   └── ListOrders/
    │   │   │       ├── ListOrdersHandler.cs
    │   │   │       └── ListOrdersResponse.cs
    │   │   ├── OrderEndpoints.cs
    │   │   └── ServiceCollectionExtensions.cs
    │   └── Customers/
    │       ├── Commands/
    │       ├── Queries/
    │       ├── CustomerEndpoints.cs
    │       └── ServiceCollectionExtensions.cs
    ├── Shared/
    │   ├── Database/
    │   │   └── DbConnectionFactory.cs
    │   └── Errors/
    │       └── ErrorTypes.cs
    └── Program.cs
```

## Handler Pattern

Handlers are simple classes with a single public method. No interfaces required unless testing demands it.

### Command Handler

```csharp
namespace MyApp.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler(
    IDbConnection db,
    IValidator<CreateOrderRequest> validator,
    ILogger<CreateOrderHandler> logger)
{
    public async Task<OneOf<CreateOrderResponse, ValidationFailed, Error>> HandleAsync(
        CreateOrderRequest request,
        CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        logger.LogInformation("Creating order for customer {CustomerId}", request.CustomerId);

        var sql = """
            INSERT INTO Orders (CustomerId, Total, CreatedAt)
            VALUES (@CustomerId, @Total, @CreatedAt)
            RETURNING Id
            """;

        var orderId = await db.ExecuteScalarAsync<int>(sql, new
        {
            request.CustomerId,
            Total = request.Items.Sum(i => i.Price * i.Quantity),
            CreatedAt = DateTime.UtcNow
        });

        return new CreateOrderResponse(orderId, DateTime.UtcNow);
    }
}
```

### Query Handler

```csharp
namespace MyApp.Features.Orders.Queries.GetOrder;

public class GetOrderHandler(IDbConnection db)
{
    public async Task<OneOf<GetOrderResponse, NotFound>> HandleAsync(
        int orderId,
        CancellationToken ct = default)
    {
        var sql = """
            SELECT o.Id, o.CustomerId, o.Total, o.CreatedAt, o.Status
            FROM Orders o
            WHERE o.Id = @OrderId
            """;

        var order = await db.QuerySingleOrDefaultAsync<GetOrderResponse>(sql, new { OrderId = orderId });

        if (order is null)
        {
            return new NotFound();
        }

        return order;
    }
}
```

## Request/Response Objects

Co-locate request and response records with their handlers:

```csharp
namespace MyApp.Features.Orders.Commands.CreateOrder;

public record CreateOrderRequest(
    string CustomerId,
    List<OrderItemRequest> Items);

public record OrderItemRequest(
    string ProductId,
    int Quantity,
    decimal Price);
```

```csharp
namespace MyApp.Features.Orders.Commands.CreateOrder;

public record CreateOrderResponse(
    int OrderId,
    DateTime CreatedAt);
```

## Service Registration

Each feature registers its own services:

```csharp
namespace MyApp.Features.Orders;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderFeature(this IServiceCollection services)
    {
        // Commands
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<CancelOrderHandler>();

        // Queries
        services.AddScoped<GetOrderHandler>();
        services.AddScoped<ListOrdersHandler>();

        // Validators
        services.AddScoped<IValidator<CreateOrderRequest>, CreateOrderValidator>();
        services.AddScoped<IValidator<CancelOrderRequest>, CancelOrderValidator>();

        return services;
    }
}
```

Register in `Program.cs`:

```csharp
builder.Services.AddOrderFeature();
builder.Services.AddCustomerFeature();
```

## Cross-Feature Communication

When features need to communicate:

1. **Prefer queries over shared state** - Call another feature's query handler
2. **Use domain events for decoupling** - When features shouldn't know about each other
3. **Extract to Shared** - For truly common concepts (e.g., `Money`, `Address`)

```csharp
// Feature A calling Feature B's query
public class SomeHandler(GetCustomerHandler getCustomer)
{
    public async Task<...> HandleAsync(...)
    {
        var customerResult = await getCustomer.HandleAsync(customerId, ct);
        // ...
    }
}
```

## Guidelines

- **Keep handlers focused** - One handler, one responsibility
- **No business logic in endpoints** - Endpoints delegate to handlers
- **Validators are required for commands** - Always validate input
- **Queries can skip validation** - Simple ID lookups don't need validators
- **Use CancellationToken** - Pass through to all async operations
- **Return OneOf results** - Make success and failure explicit in the type system
