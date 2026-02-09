using Questline.Domain;

namespace Questline.Tests.Domain;

public class InventoryTests
{
    [Fact]
    public void Add_ThenFindByName_ReturnsItem()
    {
        var inventory = new Inventory();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };

        inventory.Add(lamp);

        inventory.FindByName("brass lamp").ShouldBe(lamp);
    }

    [Fact]
    public void FindByName_IsCaseInsensitive()
    {
        var inventory = new Inventory();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        inventory.Add(lamp);

        inventory.FindByName("BRASS LAMP").ShouldBe(lamp);
    }

    [Fact]
    public void FindByName_WhenNotFound_ReturnsNull()
    {
        var inventory = new Inventory();

        inventory.FindByName("sword").ShouldBeNull();
    }

    [Fact]
    public void Remove_WhenItemExists_ReturnsTrueAndRemovesItem()
    {
        var inventory = new Inventory();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        inventory.Add(lamp);

        inventory.Remove(lamp).ShouldBeTrue();
        inventory.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void IsEmpty_WhenNoItems_ReturnsTrue()
    {
        var inventory = new Inventory();

        inventory.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void Items_ReturnsReadonlyCollection()
    {
        var inventory = new Inventory();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        inventory.Add(lamp);

        inventory.Items.ShouldContain(lamp);
    }

    [Fact]
    public void Contains_WhenItemPresent_ReturnsTrue()
    {
        var inventory = new Inventory();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        inventory.Add(lamp);

        inventory.Contains(lamp).ShouldBeTrue();
    }

    [Fact]
    public void Contains_WhenItemAbsent_ReturnsFalse()
    {
        var inventory = new Inventory();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };

        inventory.Contains(lamp).ShouldBeFalse();
    }
}
