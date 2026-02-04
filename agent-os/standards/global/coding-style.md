# Coding Style

## Modern C# Features

All projects MUST use modern C# features:

### Nullable Reference Types
Always enabled project-wide:
```xml
<PropertyGroup>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

### File-Scoped Namespaces
Use file-scoped namespaces to reduce nesting:
```csharp
namespace MyApp.Features.Orders;

public class CreateOrderHandler
{
    // ...
}
```

### Primary Constructors
Prefer primary constructors for dependency injection:
```csharp
public class OrderService(IDbConnection db, ILogger<OrderService> logger)
{
    public async Task<Order> GetOrderAsync(int id)
    {
        logger.LogInformation("Fetching order {OrderId}", id);
        return await db.QuerySingleAsync<Order>("SELECT * FROM Orders WHERE Id = @Id", new { Id = id });
    }
}
```

### Records
Use records for:
- DTOs and request/response objects
- Value objects
- Immutable data structures

```csharp
// Request/Response
public record CreateOrderRequest(string CustomerId, List<OrderItem> Items);
public record CreateOrderResponse(int OrderId, DateTime CreatedAt);

// Value objects
public record Money(decimal Amount, string Currency);
public record Address(string Street, string City, string PostCode);
```

### Collection Expressions
Use collection expressions for initialization:
```csharp
List<string> names = ["Alice", "Bob", "Charlie"];
int[] numbers = [1, 2, 3, 4, 5];
```

### Pattern Matching
Use pattern matching for cleaner conditionals:
```csharp
return result switch
{
    Success<Order> s => Results.Ok(s.Value),
    NotFound => Results.NotFound(),
    ValidationFailed v => Results.BadRequest(v.Errors),
    _ => Results.Problem()
};
```

### Raw String Literals
Use raw string literals for SQL and multi-line strings:
```csharp
var sql = """
    SELECT o.Id, o.CustomerId, o.Total
    FROM Orders o
    WHERE o.Status = @Status
    ORDER BY o.CreatedAt DESC
    """;
```

## Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Classes | PascalCase | `OrderService` |
| Interfaces | IPascalCase | `IOrderRepository` |
| Methods | PascalCase | `GetOrderAsync` |
| Properties | PascalCase | `OrderId` |
| Private fields | _camelCase | `_orderService` |
| Parameters | camelCase | `orderId` |
| Local variables | camelCase | `orderTotal` |
| Constants | PascalCase | `MaxRetryCount` |
| Async methods | Suffix with Async | `GetOrderAsync` |

## EditorConfig

All projects must include `.editorconfig` for consistent formatting:

```ini
root = true

[*]
indent_style = space
indent_size = 4
end_of_line = lf
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true

[*.cs]
# Organize usings
dotnet_sort_system_directives_first = true
dotnet_separate_import_directive_groups = false

# this. preferences
dotnet_style_qualification_for_field = false:warning
dotnet_style_qualification_for_property = false:warning
dotnet_style_qualification_for_method = false:warning
dotnet_style_qualification_for_event = false:warning

# Use language keywords
dotnet_style_predefined_type_for_locals_parameters_members = true:warning
dotnet_style_predefined_type_for_member_access = true:warning

# Modifier preferences
dotnet_style_require_accessibility_modifiers = for_non_interface_members:warning
csharp_preferred_modifier_order = public,private,protected,internal,static,extern,new,virtual,abstract,sealed,override,readonly,unsafe,volatile,async:warning

# Expression preferences
dotnet_style_object_initializer = true:suggestion
dotnet_style_collection_initializer = true:suggestion
dotnet_style_prefer_auto_properties = true:suggestion
dotnet_style_prefer_conditional_expression_over_assignment = true:suggestion

# Null checking
dotnet_style_coalesce_expression = true:warning
dotnet_style_null_propagation = true:warning
dotnet_style_prefer_is_null_check_over_reference_equality_method = true:warning

# C# style
csharp_style_var_for_built_in_types = false:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_var_elsewhere = true:suggestion
csharp_style_expression_bodied_methods = when_on_single_line:suggestion
csharp_style_expression_bodied_properties = true:suggestion
csharp_style_expression_bodied_constructors = false:suggestion
csharp_style_pattern_matching_over_is_with_cast_check = true:warning
csharp_style_pattern_matching_over_as_with_null_check = true:warning
csharp_style_namespace_declarations = file_scoped:warning
csharp_style_prefer_primary_constructors = true:suggestion

# New line preferences
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true

# Indentation
csharp_indent_case_contents = true
csharp_indent_switch_labels = true

# Spacing
csharp_space_after_cast = false
csharp_space_after_keywords_in_control_flow_statements = true
csharp_space_between_method_declaration_parameter_list_parentheses = false
csharp_space_between_method_call_parameter_list_parentheses = false
```

## Code Organization

### File Structure
- One type per file (exceptions: nested classes, related records)
- File name matches type name
- Group related files in feature folders

### Using Directives
- Place at top of file (not inside namespace)
- Sort alphabetically with System namespaces first
- Remove unused usings

### Method Ordering
1. Public methods
2. Internal methods
3. Protected methods
4. Private methods

Within each group, order by logical flow or alphabetically.
