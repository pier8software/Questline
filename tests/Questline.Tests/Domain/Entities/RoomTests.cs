using Questline.Domain.Entities;

namespace Questline.Tests.Domain.Entities;

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
            Exits = new Dictionary<Direction, Exit>
            {
                [Direction.North] = new Exit("throne-room"),
                [Direction.South] = new Exit("entrance")
            }
        };

        room.Exits.ShouldContainKey(Direction.North);
        room.Exits[Direction.North].Destination.ShouldBe("throne-room");
        room.Exits.ShouldContainKey(Direction.South);
        room.Exits[Direction.South].Destination.ShouldBe("entrance");
    }
}
