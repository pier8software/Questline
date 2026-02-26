using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;

namespace Questline.Tests.Domain.Rooms.Entity;

public class RoomTests
{
    [Fact]
    public void Can_add_an_item_to_a_rooms_inventory()
    {
        var lamp = new Item { Id = "lamp", Name   = "brass lamp", Description = "A shiny brass lamp." };
        var room = new Room { Id = "cellar", Name = "Cellar", Description     = "A damp cellar." };

        room.AddItem(lamp);

        room.FindItemByName("brass lamp").ShouldBe(lamp);
    }

    [Fact]
    public void Can_remove_an_item_from_a_rooms_inventory()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var room = new Room
        {
            Id          = "cellar",
            Name        = "Cellar",
            Description = "A damp cellar.",
            Items       = new List<Item> { lamp }
        };

        room.RemoveItem(lamp);

        room.Items.ShouldBeEmpty();
    }

    [Fact]
    public void FindItemByName_returns_item_case_insensitively()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var room = new Room
        {
            Id          = "cellar",
            Name        = "Cellar",
            Description = "A damp cellar.",
            Items       = new List<Item> { lamp }
        };

        room.FindItemByName("BRASS LAMP").ShouldBe(lamp);
    }

    [Fact]
    public void FindItemByName_returns_null_when_not_found()
    {
        var room = new Room { Id = "cellar", Name = "Cellar", Description = "A damp cellar." };

        room.FindItemByName("sword").ShouldBeNull();
    }
}
