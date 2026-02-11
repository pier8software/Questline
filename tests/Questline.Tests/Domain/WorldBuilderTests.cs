namespace Questline.Domain;

public class WorldBuilderTests
{
    [Fact]
    public void Builds_world_with_rooms()
    {
        var world = new WorldBuilder()
            .WithRoom("entrance", "Entrance", "The dungeon entrance.")
            .Build();

        var room = world.GetRoom("entrance");
        room.Name.ShouldBe("Entrance");
        room.Description.ShouldBe("The dungeon entrance.");
    }

    [Fact]
    public void Exits_connect_rooms_bidirectionally()
    {
        var world = new WorldBuilder()
            .WithRoom("start", "Start", "Starting room.", r => { r.WithExit(Direction.North, "end"); })
            .WithRoom("end", "End", "Ending room.", r => r.WithExit(Direction.South, "start"))
            .Build();

        var start = world.GetRoom("start");
        start.Exits.ShouldContainKey(Direction.North);
        start.Exits[Direction.North].Destination.ShouldBe("end");

        var end = world.GetRoom("end");
        end.Exits.ShouldContainKey(Direction.South);
        end.Exits[Direction.South].Destination.ShouldBe("start");
    }

    [Fact]
    public void Items_added_to_room_are_present()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();

        var room = world.GetRoom("cellar");
        room.Items.FindByName("brass lamp").ShouldBe(lamp);
    }
}
