namespace Questline.Domain;

public class RoomTests
{
    [Fact]
    public void Room_HasRequiredProperties()
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
    public void Room_HasEmptyExitsByDefault()
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
    public void Room_CanHaveExitsLinkingToOtherRooms()
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
