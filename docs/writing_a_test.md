# Writing a Test

## Test First, Always

No production code without a failing test.

## Stack

- **xUnit** for test framework
- **Shouldly** for assertions

## Naming Convention

Plain English, fact-based naming. Test names are declarative statements describing behaviour from a business perspective.

### Format

```csharp
public class <Subject>Tests
{
    [Fact]
    public void <Behaviour_described_as_fact>() { }
}
```

### Examples

```csharp
public class InventoryTests
{
    [Fact]
    public void Added_item_is_found_by_name() { }

    [Fact]
    public void Find_by_name_is_case_insensitive() { }

    [Fact]
    public void New_inventory_is_empty() { }
}
```

Use underscores to separate words. Start with the outcome or state, not "Should" or "Test".

## Test Structure

Follow Arrange-Act-Assert:

```csharp
[Fact]
public void Added_item_is_found_by_name()
{
    // Arrange
    var inventory = new Inventory();
    var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };

    // Act
    inventory.Add(lamp);

    // Assert
    inventory.FindByName("brass lamp").ShouldBe(lamp);
}
```

## Assertions with Shouldly

Prefer Shouldly's fluent assertions:

```csharp
result.ShouldBe(expected);
collection.ShouldContain(item);
value.ShouldBeNull();
flag.ShouldBeTrue();
action.ShouldThrow<InvalidOperationException>();
```

## Test Fakes

Place test doubles alongside tests in the same namespace:

- `FakeConsole` in `Questline.Tests.Cli/`

Keep fakes minimal — only implement what tests require.

## Spec-Driven Tests

Each spec requirement should have corresponding tests. Map scenarios to test methods:

| Spec Scenario | Test Method |
|---------------|-------------|
| `WHEN an Item is added THEN FindByName returns it` | `Added_item_is_found_by_name()` |
| `WHEN Inventory is empty THEN IsEmpty is true` | `New_inventory_is_empty()` |

## Before Committing

- [ ] All tests pass (`dotnet test`)
- [ ] No `Console.WriteLine` debugging left behind
- [ ] No commented-out code
