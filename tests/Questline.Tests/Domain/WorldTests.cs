namespace Questline.Domain;

public class WorldTests
{
    [Fact]
    public void WorldBuilder_BuildsWorldWithRooms()
    {
        var world = new WorldBuilder()
            .WithRoom("entrance", "Entrance", "The dungeon entrance.")
            .Build();

        var room = world.GetRoom("entrance");
        room.Name.ShouldBe("Entrance");
        room.Description.ShouldBe("The dungeon entrance.");
    }

    [Fact]
    public void WorldBuilder_WithExit_ConnectsRooms()
    {
        var world = new WorldBuilder()
            .WithRoom("start", "Start", "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end", "End", "Ending room.", r => r.WithExit(Direction.South, "start"))
            .Build();

        var start = world.GetRoom("start");
        start.Exits.ShouldContainKey(Direction.North);
        start.Exits[Direction.North].ShouldBe("end");

        var end = world.GetRoom("end");
        end.Exits.ShouldContainKey(Direction.South);
        end.Exits[Direction.South].ShouldBe("start");
    }

    [Fact]
    public void World_GetRoom_ThrowsForUnknownRoom()
    {
        var world = new WorldBuilder()
            .WithRoom("only-room", "Only Room", "The only room.")
            .Build();

        Should.Throw<KeyNotFoundException>(() => world.GetRoom("nonexistent"));
    }

    [Fact]
    public void Player_HasMutableLocation()
    {
        var player = new Player { Id = "player1", Location = "start" };
        player.Location.ShouldBe("start");

        player.Location = "end";
        player.Location.ShouldBe("end");
    }

    [Fact]
    public void GameState_HoldsWorldAndPlayer()
    {
        var world = new WorldBuilder()
            .WithRoom("start", "Start", "Starting room.")
            .Build();
        var player = new Player { Id = "player1", Location = "start" };

        var state = new GameState(world, player);

        state.World.ShouldBeSameAs(world);
        state.Player.ShouldBeSameAs(player);
    }
}
