# Data Access with DynamoDB

## Core Principles

1. **Repository per entity** - Each entity type has its own repository
2. **Document Model** - Repositories use Table/Document classes internally
3. **Encapsulated mapping** - Repositories convert between entities and Documents
4. **Three core methods** - `Load`, `Save`, `Query`

## Repository Pattern

### Base Structure

```csharp
public class OrderRepository(IAmazonDynamoDB client)
{
    private readonly Table _table = Table.LoadTable(client, "Orders");

    public async Task<Order?> LoadAsync(string id, CancellationToken ct = default)
    {
        var doc = await _table.GetItemAsync(id, ct);
        return doc is null ? null : MapToEntity(doc);
    }

    public async Task SaveAsync(Order order, CancellationToken ct = default)
    {
        var doc = MapToDocument(order);
        await _table.PutItemAsync(doc, ct);
    }

    public async Task<IEnumerable<Order>> QueryAsync(
        QueryFilter filter,
        string? indexName = null,
        CancellationToken ct = default)
    {
        var config = new QueryOperationConfig { Filter = filter };
        if (indexName is not null)
        {
            config.IndexName = indexName;
        }

        var search = _table.Query(config);
        var results = new List<Order>();

        do
        {
            var docs = await search.GetNextSetAsync(ct);
            results.AddRange(docs.Select(MapToEntity));
        } while (!search.IsDone);

        return results;
    }

    private static Order MapToEntity(Document doc) => new()
    {
        Id = doc["Id"],
        CustomerId = doc["CustomerId"],
        Total = doc["Total"].AsDecimal(),
        Status = doc["Status"],
        CreatedAt = DateTime.Parse(doc["CreatedAt"]),
        UpdatedAt = doc.ContainsKey("UpdatedAt")
            ? DateTime.Parse(doc["UpdatedAt"])
            : null
    };

    private static Document MapToDocument(Order order) => new()
    {
        ["Id"] = order.Id,
        ["CustomerId"] = order.CustomerId,
        ["Total"] = order.Total,
        ["Status"] = order.Status,
        ["CreatedAt"] = order.CreatedAt.ToString("O"),
        ["UpdatedAt"] = order.UpdatedAt?.ToString("O")
    };
}
```

## Handler Usage

```csharp
public class CreateOrderHandler(
    OrderRepository orders,
    IValidator<CreateOrderRequest> validator)
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

        var order = new Order
        {
            Id = Ulid.NewUlid().ToString(),
            CustomerId = request.CustomerId,
            Total = request.Items.Sum(i => i.Price * i.Quantity),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await orders.SaveAsync(order, ct);

        return new CreateOrderResponse(order.Id, order.CreatedAt);
    }
}
```

## Query Helpers

Add convenience methods for common access patterns:

```csharp
public class OrderRepository(IAmazonDynamoDB client)
{
    // ... core methods ...

    public Task<IEnumerable<Order>> QueryByCustomerAsync(
        string customerId,
        CancellationToken ct = default)
    {
        var filter = new QueryFilter("CustomerId", QueryOperator.Equal, customerId);
        return QueryAsync(filter, "CustomerId-index", ct);
    }

    public Task<IEnumerable<Order>> QueryByStatusAsync(
        string status,
        CancellationToken ct = default)
    {
        var filter = new QueryFilter("Status", QueryOperator.Equal, status);
        return QueryAsync(filter, "Status-index", ct);
    }
}
```

## Transactions

Use `TransactWriteItemsAsync` for atomic operations across tables:

```csharp
public class TransferHandler(IAmazonDynamoDB client)
{
    public async Task TransferAsync(
        string fromAccountId,
        string toAccountId,
        decimal amount,
        CancellationToken ct = default)
    {
        var request = new TransactWriteItemsRequest
        {
            TransactItems =
            [
                new TransactWriteItem
                {
                    Update = new Update
                    {
                        TableName = "Accounts",
                        Key = new Dictionary<string, AttributeValue>
                        {
                            ["Id"] = new(fromAccountId)
                        },
                        UpdateExpression = "SET Balance = Balance - :amount",
                        ConditionExpression = "Balance >= :amount",
                        ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                        {
                            [":amount"] = new() { N = amount.ToString() }
                        }
                    }
                },
                new TransactWriteItem
                {
                    Update = new Update
                    {
                        TableName = "Accounts",
                        Key = new Dictionary<string, AttributeValue>
                        {
                            ["Id"] = new(toAccountId)
                        },
                        UpdateExpression = "SET Balance = Balance + :amount",
                        ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                        {
                            [":amount"] = new() { N = amount.ToString() }
                        }
                    }
                }
            ]
        };

        await client.TransactWriteItemsAsync(request, ct);
    }
}
```

## Service Registration

```csharp
// Program.cs
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonDynamoDB>();

// Repositories
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<CustomerRepository>();
```

## Table Design

### Naming Convention

```
{EntityName}s  (plural)

Examples:
Orders
Customers
Products
```

### Global Secondary Indexes

Create indexes for query access patterns:

```bash
awslocal dynamodb create-table \
  --table-name Orders \
  --attribute-definitions \
    AttributeName=Id,AttributeType=S \
    AttributeName=CustomerId,AttributeType=S \
    AttributeName=Status,AttributeType=S \
  --key-schema AttributeName=Id,KeyType=HASH \
  --global-secondary-indexes \
    '[{"IndexName":"CustomerId-index","KeySchema":[{"AttributeName":"CustomerId","KeyType":"HASH"}],"Projection":{"ProjectionType":"ALL"}},
      {"IndexName":"Status-index","KeySchema":[{"AttributeName":"Status","KeyType":"HASH"}],"Projection":{"ProjectionType":"ALL"}}]' \
  --billing-mode PAY_PER_REQUEST
```

## Guidelines

- **One repository per entity** - Keep mapping logic encapsulated
- **Use indexes for queries** - Avoid full table scans
- **Use ULID for IDs** - Sortable, URL-safe, and unique
- **ISO 8601 for dates** - Store as strings with `.ToString("O")`
- **Handle missing attributes** - Check `ContainsKey` for optional fields
- **Use transactions for consistency** - When modifying multiple items atomically
