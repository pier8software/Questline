using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Domain.Rooms.Entity;

public class RoomTests
{
    [Fact]
    public void Room_stores_items_as_read_only()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var room = new Room
        {
            Id          = "cellar",
            Name        = "Cellar",
            Description = "A damp cellar.",
            Items       = [lamp]
        };

        room.Items.ShouldContain(lamp);
    }

    [Fact]
    public void Room_stores_exits()
    {
        var room = new Room
        {
            Id          = "hallway",
            Name        = "Hallway",
            Description = "A long hallway.",
            Exits       = new Dictionary<Direction, Exit>
            {
                [Direction.North] = new("throne-room"),
                [Direction.South] = new("entrance")
            }
        };

        room.Exits.ShouldContainKey(Direction.North);
        room.Exits.ShouldContainKey(Direction.South);
    }

    [Fact]
    public void Exit_can_embed_barrier()
    {
        var barrier = new Barrier
        {
            Id             = "iron-door",
            Name           = "iron door",
            Description    = "A heavy iron door.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId   = "rusty-key",
            UnlockMessage  = "The rusty key turns in the lock..."
        };

        var exit = new Exit("beyond", barrier);

        exit.Barrier.ShouldNotBeNull();
        exit.Barrier!.Id.ShouldBe("iron-door");
    }
}
