using System.Collections.Immutable;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;

namespace Questline.Tests.Domain.Rooms.Entity;

public class RoomTests
{
    [Fact]
    public void Has_required_id_name_and_description()
    {
        var room = new Room
        {
            Id = "tavern",
            Name = "The Tavern",
            Description = "A cozy tavern with a roaring fire."
        };

        room.Id.ShouldBe("tavern");
        room.Name.ShouldBe("The Tavern");
        room.Description.ShouldBe("A cozy tavern with a roaring fire.");
    }

    [Fact]
    public void Has_empty_exits_by_default()
    {
        var room = new Room
        {
            Id = "cell",
            Name = "Cell",
            Description = "A dark cell."
        };

        room.Exits.ShouldBeEmpty();
    }

    [Fact]
    public void Exits_link_to_other_rooms()
    {
        var room = new Room
        {
            Id = "hallway",
            Name = "Hallway",
            Description = "A long hallway.",
            Exits = ImmutableDictionary.CreateRange(new Dictionary<Direction, Exit>
            {
                [Direction.North] = new("throne-room"),
                [Direction.South] = new("entrance")
            })
        };

        room.Exits.ShouldContainKey(Direction.North);
        room.Exits[Direction.North].Destination.ShouldBe("throne-room");
        room.Exits.ShouldContainKey(Direction.South);
        room.Exits[Direction.South].Destination.ShouldBe("entrance");
    }

    [Fact]
    public void AddItem_returns_new_room_with_item()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var room = new Room { Id = "cellar", Name = "Cellar", Description = "A damp cellar." };

        var updated = room.AddItem(lamp);

        updated.Items.FindByName("brass lamp").ShouldBe(lamp);
        room.Items.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void RemoveItem_returns_new_room_without_item()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var room = new Room
        {
            Id = "cellar",
            Name = "Cellar",
            Description = "A damp cellar.",
            Items = new Inventory().Add(lamp)
        };

        var updated = room.RemoveItem(lamp);

        updated.Items.IsEmpty.ShouldBeTrue();
        room.Items.FindByName("brass lamp").ShouldBe(lamp);
    }
}
