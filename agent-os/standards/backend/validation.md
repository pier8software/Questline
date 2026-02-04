# Validation with FluentValidation

## Core Principles

1. **Validate at the boundary** - Validate requests before processing
2. **Co-locate validators with features** - Validator lives in the same folder as the handler
3. **Return validation errors, don't throw** - Use OneOf to make validation failures explicit
4. **Validators are required for commands** - Every command request needs a validator

## Validator Structure

### Basic Validator

```csharp
namespace MyApp.Features.Orders.Commands.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Order must contain at least one item");

        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemValidator());
    }
}

public class OrderItemValidator : AbstractValidator<OrderItemRequest>
{
    public OrderItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price cannot be negative");
    }
}
```

## Common Validation Rules

### String Validation

```csharp
RuleFor(x => x.Name)
    .NotEmpty()
    .MaximumLength(100)
    .Matches(@"^[a-zA-Z\s]+$")
    .WithMessage("Name can only contain letters and spaces");

RuleFor(x => x.Email)
    .NotEmpty()
    .EmailAddress();

RuleFor(x => x.Url)
    .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
    .When(x => !string.IsNullOrEmpty(x.Url))
    .WithMessage("Must be a valid URL");
```

### Numeric Validation

```csharp
RuleFor(x => x.Age)
    .InclusiveBetween(18, 120);

RuleFor(x => x.Amount)
    .GreaterThan(0)
    .PrecisionScale(10, 2, ignoreTrailingZeros: true);
```

### Collection Validation

```csharp
RuleFor(x => x.Tags)
    .NotEmpty()
    .Must(tags => tags.Count <= 10)
    .WithMessage("Maximum 10 tags allowed");

RuleForEach(x => x.Tags)
    .NotEmpty()
    .MaximumLength(50);
```

### Conditional Validation

```csharp
RuleFor(x => x.ShippingAddress)
    .NotNull()
    .When(x => x.DeliveryMethod == DeliveryMethod.Shipping);

RuleFor(x => x.PickupLocation)
    .NotEmpty()
    .When(x => x.DeliveryMethod == DeliveryMethod.Pickup);
```

### Custom Validation

```csharp
RuleFor(x => x.StartDate)
    .Must(date => date > DateTime.UtcNow)
    .WithMessage("Start date must be in the future");

RuleFor(x => x.EndDate)
    .GreaterThan(x => x.StartDate)
    .WithMessage("End date must be after start date");
```

## Async Validation

For validation that requires database or external service calls:

```csharp
public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator(IDbConnection db)
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .MustAsync(async (customerId, ct) =>
            {
                var exists = await db.ExecuteScalarAsync<bool>(
                    "SELECT EXISTS(SELECT 1 FROM Customers WHERE Id = @Id)",
                    new { Id = customerId });
                return exists;
            })
            .WithMessage("Customer not found");
    }
}
```

## Integration with Handlers

### Handler Pattern

```csharp
public class CreateOrderHandler(
    IDbConnection db,
    IValidator<CreateOrderRequest> validator,
    ILogger<CreateOrderHandler> logger)
{
    public async Task<OneOf<CreateOrderResponse, ValidationFailed, Error>> HandleAsync(
        CreateOrderRequest request,
        CancellationToken ct = default)
    {
        // Validate first
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        // Proceed with business logic
        // ...
    }
}
```

### Validation Error Type

```csharp
namespace MyApp.Shared.Errors;

public record ValidationFailed(IEnumerable<ValidationFailure> Errors)
{
    public ValidationFailed(params ValidationFailure[] errors) : this(errors.AsEnumerable()) { }
}
```

### Mapping to API Response

```csharp
private static async Task<IResult> CreateOrder(
    CreateOrderRequest request,
    CreateOrderHandler handler,
    CancellationToken ct)
{
    var result = await handler.HandleAsync(request, ct);

    return result.Match<IResult>(
        success => Results.Created($"/api/orders/{success.OrderId}", success),
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
```

## Service Registration

```csharp
// Per feature
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderFeature(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateOrderRequest>, CreateOrderValidator>();
        services.AddScoped<IValidator<UpdateOrderRequest>, UpdateOrderValidator>();
        // ...
        return services;
    }
}
```

Or register all validators from assembly:

```csharp
// Program.cs - only if you prefer convention-based registration
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
```

## Guidelines

- **Validate early** - Check input before any business logic
- **Use descriptive error messages** - Help users understand what went wrong
- **Group related rules** - Use child validators for complex nested objects
- **Avoid business logic in validators** - Validators check input format, not business rules
- **Make validators stateless when possible** - Inject dependencies only for async validation
- **Test validators independently** - Validators are easy to unit test
