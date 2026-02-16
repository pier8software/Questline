using Questline.Domain.Shared.Entity;

namespace Questline.Tests.Domain.Shared;

public class InventoryTests
{
    [Fact]
    public void Added_item_is_found_by_name()
    {
        var inventory = new Inventory();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };

        inventory.Add(lamp);

        inventory.FindByName("brass lamp").ShouldBe(lamp);
    }

    [Fact]
    public void Find_by_name_is_case_insensitive()
    {
        var inventory = new Inventory();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        inventory.Add(lamp);

        inventory.FindByName("BRASS LAMP").ShouldBe(lamp);
    }

    [Fact]
    public void Find_by_name_returns_null_when_item_not_present()
    {
        var inventory = new Inventory();

        inventory.FindByName("sword").ShouldBeNull();
    }

    [Fact]
    public void Removing_an_item_makes_inventory_empty()
    {
        var inventory = new Inventory();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        inventory.Add(lamp);

        inventory.Remove(lamp).ShouldBeTrue();
        inventory.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void New_inventory_is_empty()
    {
        var inventory = new Inventory();

        inventory.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void Items_collection_contains_added_item()
    {
        var inventory = new Inventory();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        inventory.Add(lamp);

        inventory.Items.ShouldContain(lamp);
    }

    [Fact]
    public void Contains_returns_true_when_item_present()
    {
        var inventory = new Inventory();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        inventory.Add(lamp);

        inventory.Contains(lamp).ShouldBeTrue();
    }

    [Fact]
    public void Contains_returns_false_when_item_absent()
    {
        var inventory = new Inventory();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };

        inventory.Contains(lamp).ShouldBeFalse();
    }
}
