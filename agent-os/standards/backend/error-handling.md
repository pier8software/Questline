# Error Handling with OneOf

## Core Principles

1. **Make errors explicit in the type system** - Return types declare all possible outcomes
2. **Don't throw exceptions for expected failures** - Use discriminated unions instead
3. **Reserve exceptions for truly exceptional cases** - Infrastructure failures, bugs
4. **Map errors to appropriate HTTP responses** - Each error type has a corresponding status code

## Error Types

Define a set of common error types in the Shared folder:

```csharp
namespace MyApp.Shared.Errors;

// Success wrapper (when you need to distinguish success from errors)
public record Success;
public record Success<T>(T Value);

// Common error types
public record NotFound(string? Message = null);

public record ValidationFailed(IEnumerable<ValidationFailure> Errors)
{
    public ValidationFailed(params ValidationFailure[] errors) : this(errors.AsEnumerable()) { }
}

public record Conflict(string Message);

public record Forbidden(string? Message = null);

public record Error(string Message, Exception? Exception = null);
```

## Handler Return Types

### Command with Multiple Outcomes

```csharp
public async Task<OneOf<CreateOrderResponse, ValidationFailed, Error>> HandleAsync(
    CreateOrderRequest request,
    CancellationToken ct = default)
{
    var validationResult = await _validator.ValidateAsync(request, ct);
    if (!validationResult.IsValid)
    {
        return new ValidationFailed(validationResult.Errors);
    }

    try
    {
        // Business logic...
        return new CreateOrderResponse(orderId, DateTime.UtcNow);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to create order");
        return new Error("Failed to create order", ex);
    }
}
```

### Query with NotFound

```csharp
public async Task<OneOf<OrderResponse, NotFound>> HandleAsync(
    int orderId,
    CancellationToken ct = default)
{
    var order = await _db.QuerySingleOrDefaultAsync<OrderResponse>(sql, new { Id = orderId });

    if (order is null)
    {
        return new NotFound();
    }

    return order;
}
```

### Business Rule Violations

```csharp
public async Task<OneOf<Success, NotFound, Conflict, Error>> HandleAsync(
    CancelOrderRequest request,
    CancellationToken ct = default)
{
    var order = await _db.QuerySingleOrDefaultAsync<Order>(
        "SELECT * FROM Orders WHERE Id = @Id",
        new { request.OrderId });

    if (order is null)
    {
        return new NotFound();
    }

    if (order.Status == "Shipped")
    {
        return new Conflict("Cannot cancel an order that has already shipped");
    }

    if (order.Status == "Cancelled")
    {
        return new Conflict("Order is already cancelled");
    }

    await _db.ExecuteAsync(
        "UPDATE Orders SET Status = 'Cancelled' WHERE Id = @Id",
        new { request.OrderId });

    return new Success();
}
```

## Mapping to HTTP Responses

### Using Match

```csharp
private static async Task<IResult> CreateOrder(
    CreateOrderRequest request,
    CreateOrderHandler handler,
    CancellationToken ct)
{
    var result = await handler.HandleAsync(request, ct);

    return result.Match<IResult>(
        success => Results.Created($"/api/orders/{success.OrderId}", success),
        validationFailed => Results.BadRequest(FormatValidationErrors(validationFailed)),
        error => Results.Problem(error.Message));
}

private static object FormatValidationErrors(ValidationFailed validationFailed)
{
    return new
    {
        Type = "validation_error",
        Errors = validationFailed.Errors.Select(e => new
        {
            Field = ToCamelCase(e.PropertyName),
            e.ErrorMessage
        })
    };
}
```

### Common Mapping Patterns

```csharp
// NotFound
result.Match<IResult>(
    order => Results.Ok(order),
    notFound => Results.NotFound());

// Conflict
result.Match<IResult>(
    success => Results.NoContent(),
    notFound => Results.NotFound(),
    conflict => Results.Conflict(new { conflict.Message }));

// Forbidden
result.Match<IResult>(
    data => Results.Ok(data),
    forbidden => Results.Forbid());

// Error (500)
result.Match<IResult>(
    success => Results.Ok(success),
    error => Results.Problem(
        detail: error.Message,
        statusCode: StatusCodes.Status500InternalServerError));
```

## Extension Methods for Common Patterns

```csharp
namespace MyApp.Shared.Errors;

public static class ResultExtensions
{
    public static IResult ToHttpResult<TSuccess>(
        this OneOf<TSuccess, NotFound> result)
    {
        return result.Match<IResult>(
            success => Results.Ok(success),
            notFound => Results.NotFound());
    }

    public static IResult ToHttpResult<TSuccess>(
        this OneOf<TSuccess, ValidationFailed, Error> result,
        Func<TSuccess, IResult>? onSuccess = null)
    {
        return result.Match(
            success => onSuccess?.Invoke(success) ?? Results.Ok(success),
            validationFailed => Results.BadRequest(new
            {
                Type = "validation_error",
                Errors = validationFailed.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    e.ErrorMessage
                })
            }),
            error => Results.Problem(error.Message));
    }
}
```

Usage:

```csharp
private static async Task<IResult> GetOrder(
    int id,
    GetOrderHandler handler,
    CancellationToken ct)
{
    var result = await handler.HandleAsync(id, ct);
    return result.ToHttpResult();
}

private static async Task<IResult> CreateOrder(
    CreateOrderRequest request,
    CreateOrderHandler handler,
    CancellationToken ct)
{
    var result = await handler.HandleAsync(request, ct);
    return result.ToHttpResult(
        success => Results.Created($"/api/orders/{success.OrderId}", success));
}
```

## When to Use Exceptions

Exceptions are still appropriate for:

1. **Infrastructure failures** - Database connection lost, network timeouts
2. **Programming errors** - Null reference, invalid operation
3. **Truly unexpected conditions** - Should never happen in normal operation

```csharp
// This is fine - infrastructure failure
try
{
    return await _db.QuerySingleAsync<Order>(sql, new { Id = orderId });
}
catch (Exception ex)
{
    _logger.LogError(ex, "Database query failed");
    return new Error("Failed to retrieve order");
}

// Don't do this - expected business case
if (order is null)
{
    throw new NotFoundException("Order not found"); // Bad!
}

// Do this instead
if (order is null)
{
    return new NotFound(); // Good!
}
```

## Guidelines

- **Define error types once** - Reuse across the application
- **Keep error messages user-friendly** - They may be shown in API responses
- **Log before returning errors** - Capture context for debugging
- **Use specific error types** - `NotFound` is better than generic `Error`
- **Don't nest OneOf types** - Keep result types flat and simple
- **Test error paths** - Ensure handlers return correct error types
