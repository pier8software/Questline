# Behavior-Driven Testing with xUnit and Shouldly

## Core Principles

1. **Test behavior, not implementation** - Focus on what the system does, not how
2. **Verify final database state** - Assert against real data in the database
3. **Verify events are published** - Use MassTransit test harness to check messages
4. **Integration over isolation** - Use real handlers with real database connections
5. **No mocking** - Test the actual behavior, not mocked interactions

## Test Infrastructure

### DatabaseFixture

```csharp
public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await MigrateDatabase();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    private async Task MigrateDatabase()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        var evolve = new Evolve.Evolve(connection, msg => { })
        {
            Locations = new[] { "db/migrations" },
            IsEraseDisabled = true,
        };

        evolve.Migrate();
    }

    public NpgsqlConnection CreateConnection() => new(ConnectionString);

    public async Task ResetDatabase()
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await connection.ExecuteAsync("""
            TRUNCATE TABLE orders, order_items, customers CASCADE;
            """);
    }
}
```

### MassTransit Test Harness Setup

```csharp
public class TestHarnessFixture : IAsyncLifetime
{
    public ITestHarness Harness { get; private set; } = null!;
    public IServiceProvider Services { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();

        services.AddMassTransitTestHarness(cfg =>
        {
            cfg.AddConsumer<OrderCreatedConsumer>();
            cfg.AddConsumer<OrderCancelledConsumer>();
        });

        Services = services.BuildServiceProvider();
        Harness = Services.GetRequiredService<ITestHarness>();
        await Harness.Start();
    }

    public async Task DisposeAsync()
    {
        await Harness.Stop();
    }
}
```

### Combined Test Fixture

```csharp
public class IntegrationTestFixture : IAsyncLifetime
{
    public DatabaseFixture Database { get; } = new();
    public ITestHarness Harness { get; private set; } = null!;
    public IServiceProvider Services { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await Database.InitializeAsync();

        var services = new ServiceCollection();

        services.AddSingleton(Database);
        services.AddScoped(_ => Database.CreateConnection());

        services.AddMassTransitTestHarness(cfg =>
        {
            cfg.AddConsumer<OrderCreatedConsumer>();
            cfg.AddConsumer<OrderCancelledConsumer>();
        });

        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<CancelOrderHandler>();
        services.AddScoped<IValidator<CreateOrderRequest>, CreateOrderValidator>();

        Services = services.BuildServiceProvider();
        Harness = Services.GetRequiredService<ITestHarness>();
        await Harness.Start();
    }

    public async Task DisposeAsync()
    {
        await Harness.Stop();
        await Database.DisposeAsync();
    }
}
```

### Base Test Class

```csharp
public abstract class IntegrationTestBase : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    protected IntegrationTestFixture Fixture { get; }
    protected IServiceScope Scope { get; private set; } = null!;
    protected NpgsqlConnection Db => Scope.ServiceProvider.GetRequiredService<NpgsqlConnection>();
    protected ITestHarness Harness => Fixture.Harness;

    protected IntegrationTestBase(IntegrationTestFixture fixture)
    {
        Fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await Fixture.Database.ResetDatabase();
        Scope = Fixture.Services.CreateScope();
        await Db.OpenAsync();
    }

    public async Task DisposeAsync()
    {
        await Db.CloseAsync();
        Scope.Dispose();
    }

    protected T GetService<T>() where T : notnull
        => Scope.ServiceProvider.GetRequiredService<T>();
}
```

## Naming Conventions

### Test Method Names

Format: `Should_ExpectedBehavior_When_Condition`

```csharp
// Good names
Should_CreateOrder_When_RequestIsValid()
Should_ReturnValidationError_When_CustomerIdIsEmpty()
Should_PublishOrderCreatedEvent_When_OrderIsCreated()
Should_UpdateOrderStatus_When_CancellingValidOrder()
Should_ReturnNotFound_When_OrderDoesNotExist()

// Bad names
Test1()
CreateOrderTest()
HandleAsync_Works()
```

### Test Class Names

Format: `{Feature}Tests` or `{Handler}Tests`

```csharp
public class CreateOrderTests : IntegrationTestBase { }
public class CancelOrderTests : IntegrationTestBase { }
public class OrderQueriesTests : IntegrationTestBase { }
```

## Test Structure

### Given-When-Then Pattern

```csharp
[Fact]
public async Task Should_CreateOrder_When_RequestIsValid()
{
    // Given
    var customer = await CreateCustomer("John Doe", "john@example.com");
    var request = new CreateOrderRequest(
        customer.Id,
        [new OrderItemRequest("product-1", 2, 25.00m)]);

    var handler = GetService<CreateOrderHandler>();

    // When
    var result = await handler.HandleAsync(request);

    // Then
    result.IsT0.ShouldBeTrue();

    var savedOrder = await Db.QuerySingleAsync<Order>(
        "SELECT * FROM orders WHERE id = @Id",
        new { Id = result.AsT0.OrderId });

    savedOrder.CustomerId.ShouldBe(customer.Id);
    savedOrder.Total.ShouldBe(50.00m);
    savedOrder.Status.ShouldBe("Pending");
}
```

## Database State Verification

### Query Patterns

```csharp
[Fact]
public async Task Should_PersistOrderWithItems_When_Created()
{
    // Given
    var customer = await CreateCustomer("Jane Doe", "jane@example.com");
    var request = new CreateOrderRequest(
        customer.Id,
        [
            new OrderItemRequest("product-1", 2, 10.00m),
            new OrderItemRequest("product-2", 1, 30.00m)
        ]);

    var handler = GetService<CreateOrderHandler>();

    // When
    var result = await handler.HandleAsync(request);

    // Then
    var order = await Db.QuerySingleAsync<Order>(
        "SELECT * FROM orders WHERE id = @Id",
        new { Id = result.AsT0.OrderId });

    order.Total.ShouldBe(50.00m);
    order.Status.ShouldBe("Pending");

    var items = await Db.QueryAsync<OrderItem>(
        "SELECT * FROM order_items WHERE order_id = @OrderId",
        new { OrderId = order.Id });

    items.Count().ShouldBe(2);
    items.ShouldContain(i => i.ProductId == "product-1" && i.Quantity == 2);
    items.ShouldContain(i => i.ProductId == "product-2" && i.Quantity == 1);
}

[Fact]
public async Task Should_UpdateOrderStatus_When_Cancelled()
{
    // Given
    var order = await CreateOrder("customer-1", "Pending", 100.00m);

    var handler = GetService<CancelOrderHandler>();
    var request = new CancelOrderRequest(order.Id, "Customer requested cancellation");

    // When
    var result = await handler.HandleAsync(request);

    // Then
    result.IsT0.ShouldBeTrue();

    var updatedOrder = await Db.QuerySingleAsync<Order>(
        "SELECT * FROM orders WHERE id = @Id",
        new { order.Id });

    updatedOrder.Status.ShouldBe("Cancelled");
    updatedOrder.CancellationReason.ShouldBe("Customer requested cancellation");
    updatedOrder.CancelledAt.ShouldNotBeNull();
}
```

### Asserting No Side Effects

```csharp
[Fact]
public async Task Should_NotModifyOrder_When_ValidationFails()
{
    // Given
    var order = await CreateOrder("customer-1", "Shipped", 100.00m);
    var originalUpdatedAt = order.UpdatedAt;

    var handler = GetService<CancelOrderHandler>();
    var request = new CancelOrderRequest(order.Id, "");  // Empty reason - invalid

    // When
    var result = await handler.HandleAsync(request);

    // Then
    result.IsT1.ShouldBeTrue();  // Validation failed

    var unchangedOrder = await Db.QuerySingleAsync<Order>(
        "SELECT * FROM orders WHERE id = @Id",
        new { order.Id });

    unchangedOrder.Status.ShouldBe("Shipped");
    unchangedOrder.UpdatedAt.ShouldBe(originalUpdatedAt);
}
```

## Event Publishing Verification

### MassTransit Test Harness Patterns

```csharp
[Fact]
public async Task Should_PublishOrderCreatedEvent_When_OrderIsCreated()
{
    // Given
    var customer = await CreateCustomer("John Doe", "john@example.com");
    var request = new CreateOrderRequest(
        customer.Id,
        [new OrderItemRequest("product-1", 2, 25.00m)]);

    var handler = GetService<CreateOrderHandler>();

    // When
    var result = await handler.HandleAsync(request);

    // Then
    result.IsT0.ShouldBeTrue();

    var published = Harness.Published
        .Select<OrderCreatedEvent>()
        .FirstOrDefault();

    published.ShouldNotBeNull();
    published.Context.Message.OrderId.ShouldBe(result.AsT0.OrderId);
    published.Context.Message.CustomerId.ShouldBe(customer.Id);
    published.Context.Message.Total.ShouldBe(50.00m);
}

[Fact]
public async Task Should_PublishOrderCancelledEvent_When_OrderIsCancelled()
{
    // Given
    var order = await CreateOrder("customer-1", "Pending", 100.00m);

    var handler = GetService<CancelOrderHandler>();
    var request = new CancelOrderRequest(order.Id, "Changed my mind");

    // When
    var result = await handler.HandleAsync(request);

    // Then
    var published = Harness.Published
        .Select<OrderCancelledEvent>()
        .FirstOrDefault();

    published.ShouldNotBeNull();
    published.Context.Message.OrderId.ShouldBe(order.Id);
    published.Context.Message.Reason.ShouldBe("Changed my mind");
}

[Fact]
public async Task Should_NotPublishEvent_When_ValidationFails()
{
    // Given
    var request = new CreateOrderRequest("", []);  // Invalid request

    var handler = GetService<CreateOrderHandler>();

    // When
    var result = await handler.HandleAsync(request);

    // Then
    result.IsT1.ShouldBeTrue();  // Validation failed

    var published = Harness.Published
        .Select<OrderCreatedEvent>()
        .Any();

    published.ShouldBeFalse();
}
```

### Checking Multiple Events

```csharp
[Fact]
public async Task Should_PublishMultipleEvents_When_BulkCancelling()
{
    // Given
    var order1 = await CreateOrder("customer-1", "Pending", 100.00m);
    var order2 = await CreateOrder("customer-1", "Pending", 200.00m);

    var handler = GetService<BulkCancelOrdersHandler>();
    var request = new BulkCancelOrdersRequest([order1.Id, order2.Id], "Bulk cancel");

    // When
    await handler.HandleAsync(request);

    // Then
    var publishedEvents = Harness.Published
        .Select<OrderCancelledEvent>()
        .ToList();

    publishedEvents.Count.ShouldBe(2);
    publishedEvents.ShouldContain(e => e.Context.Message.OrderId == order1.Id);
    publishedEvents.ShouldContain(e => e.Context.Message.OrderId == order2.Id);
}
```

## Test Builders

Use builders for setting up test data:

```csharp
public class OrderBuilder
{
    private string _customerId = "customer-1";
    private string _status = "Pending";
    private decimal _total = 100.00m;
    private readonly List<(string ProductId, int Quantity, decimal Price)> _items = [];

    public OrderBuilder WithCustomer(string customerId)
    {
        _customerId = customerId;
        return this;
    }

    public OrderBuilder WithStatus(string status)
    {
        _status = status;
        return this;
    }

    public OrderBuilder WithTotal(decimal total)
    {
        _total = total;
        return this;
    }

    public OrderBuilder WithItem(string productId, int quantity, decimal price)
    {
        _items.Add((productId, quantity, price));
        return this;
    }

    public async Task<Order> BuildAsync(NpgsqlConnection db)
    {
        var orderId = await db.ExecuteScalarAsync<int>("""
            INSERT INTO orders (customer_id, status, total, created_at)
            VALUES (@CustomerId, @Status, @Total, NOW())
            RETURNING id
            """,
            new { CustomerId = _customerId, Status = _status, Total = _total });

        foreach (var item in _items)
        {
            await db.ExecuteAsync("""
                INSERT INTO order_items (order_id, product_id, quantity, price)
                VALUES (@OrderId, @ProductId, @Quantity, @Price)
                """,
                new { OrderId = orderId, item.ProductId, item.Quantity, item.Price });
        }

        return await db.QuerySingleAsync<Order>(
            "SELECT * FROM orders WHERE id = @Id",
            new { Id = orderId });
    }
}

// Usage in tests
var order = await new OrderBuilder()
    .WithCustomer("vip-customer")
    .WithStatus("Pending")
    .WithItem("product-1", 2, 25.00m)
    .WithItem("product-2", 1, 50.00m)
    .BuildAsync(Db);
```

## Helper Methods in Base Class

```csharp
public abstract class IntegrationTestBase : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    // ... fixture setup ...

    protected async Task<Customer> CreateCustomer(string name, string email)
    {
        var id = await Db.ExecuteScalarAsync<int>("""
            INSERT INTO customers (name, email, created_at)
            VALUES (@Name, @Email, NOW())
            RETURNING id
            """,
            new { Name = name, Email = email });

        return new Customer { Id = id, Name = name, Email = email };
    }

    protected async Task<Order> CreateOrder(string customerId, string status, decimal total)
    {
        return await new OrderBuilder()
            .WithCustomer(customerId)
            .WithStatus(status)
            .WithTotal(total)
            .BuildAsync(Db);
    }

    protected async Task<Order> CreateOrderWithItems(
        string customerId,
        params (string ProductId, int Quantity, decimal Price)[] items)
    {
        var builder = new OrderBuilder().WithCustomer(customerId);
        foreach (var item in items)
        {
            builder.WithItem(item.ProductId, item.Quantity, item.Price);
        }
        return await builder.BuildAsync(Db);
    }
}
```

## File Organization

```
tests/
└── MyApp.Tests/
    ├── Features/
    │   ├── Orders/
    │   │   ├── CreateOrderTests.cs
    │   │   ├── CancelOrderTests.cs
    │   │   ├── GetOrderTests.cs
    │   │   └── ListOrdersTests.cs
    │   └── Customers/
    │       └── ...
    ├── Infrastructure/
    │   ├── IntegrationTestFixture.cs
    │   ├── IntegrationTestBase.cs
    │   └── DatabaseFixture.cs
    ├── Builders/
    │   ├── OrderBuilder.cs
    │   └── CustomerBuilder.cs
    └── MyApp.Tests.csproj
```

## Guidelines

- **No mocking** - Use real implementations with real database
- **Reset database between tests** - Ensure test isolation
- **Use builders for test data** - Keep arrange sections clean
- **Assert both database and events** - Verify complete behavior
- **Keep tests focused** - One behavior per test
- **Use meaningful test names** - Should-style clearly describes behavior
- **Test error paths** - Verify validation failures don't cause side effects
- **Use transactions sparingly** - Let the handler manage its own transactions

```csharp
[Theory]
[InlineData("")]
[InlineData(" ")]
[InlineData(null)]
public async Task Should_ReturnValidationError_When_CustomerIdIsInvalid(string? customerId)
{
    // Given
    var request = new CreateOrderRequest(customerId!, [new("p1", 1, 10)]);
    var handler = GetService<CreateOrderHandler>();

    // When
    var result = await handler.HandleAsync(request);

    // Then
    result.IsT1.ShouldBeTrue();
    result.AsT1.Errors.ShouldContain(e => e.PropertyName == "CustomerId");

    var orderCount = await Db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM orders");
    orderCount.ShouldBe(0);
}
```
