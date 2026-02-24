using System.Collections.Immutable;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;

namespace Questline.Tests.Domain.Rooms.Entity;

public class RoomTests
{
    [Fact]
    public void Can_add_an_item_to_a_rooms_inventory()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var room = new Room { Id = "cellar", Name = "Cellar", Description = "A damp cellar." };

        room = room.AddItem(lamp);

        room.Items.FindByName("brass lamp").ShouldBe(lamp);
    }

    [Fact]
    public void Can_remove_an_item_from_a_rooms_inventory()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var room = new Room
        {
            Id = "cellar",
            Name = "Cellar",
            Description = "A damp cellar.",
            Items = new Inventory().Add(lamp)
        };

        room = room.RemoveItem(lamp);

        room.Items.IsEmpty.ShouldBeTrue();
    }
}
