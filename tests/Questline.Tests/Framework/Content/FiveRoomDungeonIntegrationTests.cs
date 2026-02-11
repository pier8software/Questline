using Questline.Domain;
using Questline.Framework.Content;

namespace Questline.Tests.Framework.Content;

public class FiveRoomDungeonIntegrationTests
{
    private static string ContentPath
    {
        get
        {
            var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "content", "adventures", "five-room-dungeon");
            return Path.GetFullPath(path);
        }
    }

    private readonly Adventure _adventure;

    public FiveRoomDungeonIntegrationTests()
    {
        var loader = new FileSystemAdventureLoader();
        _adventure = loader.Load(ContentPath);
    }

    [Fact]
    public void Load_AdventureMetadata_IsCorrect()
    {
        _adventure.Id.ShouldBe("five-room-dungeon");
        _adventure.Name.ShouldBe("The Goblin's Lair");
        _adventure.StartingRoomId.ShouldBe("entrance");
    }

    [Fact]
    public void Load_AllFiveRoomsExist()
    {
        var roomIds = new[] { "entrance", "puzzle-room", "setback-room", "climax-room", "treasure-room" };

        foreach (var id in roomIds)
        {
            Should.NotThrow(() => _adventure.World.GetRoom(id));
        }
    }

    [Fact]
    public void Load_EntranceRoom_HasCorrectExitsAndItems()
    {
        var room = _adventure.World.GetRoom("entrance");

        room.Name.ShouldBe("Cave Entrance");
        room.Exits.ShouldContainKey(Direction.North);
        room.Exits[Direction.North].Destination.ShouldBe("puzzle-room");
        room.Items.Items.ShouldContain(i => i.Id == "rusty-key");
    }

    [Fact]
    public void Load_PuzzleRoom_HasBarrierExitDestination()
    {
        var room = _adventure.World.GetRoom("puzzle-room");

        room.Exits.ShouldContainKey(Direction.North);
        room.Exits[Direction.North].Destination.ShouldBe("setback-room");
        room.Exits[Direction.North].BarrierId.ShouldBe("iron-door");
        room.Exits.ShouldContainKey(Direction.South);
        room.Items.Items.ShouldContain(i => i.Id == "torch");
    }

    [Fact]
    public void Load_AllItemsPlacedCorrectly()
    {
        _adventure.World.GetRoom("entrance").Items.Items
            .ShouldContain(i => i.Id == "rusty-key");
        _adventure.World.GetRoom("puzzle-room").Items.Items
            .ShouldContain(i => i.Id == "torch");
        _adventure.World.GetRoom("setback-room").Items.Items
            .ShouldContain(i => i.Id == "broken-sword");
        _adventure.World.GetRoom("climax-room").Items.Items
            .ShouldContain(i => i.Id == "goblin-idol");
        _adventure.World.GetRoom("treasure-room").Items.Items
            .ShouldContain(i => i.Id == "gold-chalice");
    }

    [Fact]
    public void Load_FullConnectivity_CanTraverseAllRooms()
    {
        var entrance = _adventure.World.GetRoom("entrance");
        entrance.Exits[Direction.North].Destination.ShouldBe("puzzle-room");

        var puzzle = _adventure.World.GetRoom("puzzle-room");
        puzzle.Exits[Direction.South].Destination.ShouldBe("entrance");
        puzzle.Exits[Direction.North].Destination.ShouldBe("setback-room");

        var setback = _adventure.World.GetRoom("setback-room");
        setback.Exits[Direction.South].Destination.ShouldBe("puzzle-room");
        setback.Exits[Direction.East].Destination.ShouldBe("climax-room");

        var climax = _adventure.World.GetRoom("climax-room");
        climax.Exits[Direction.West].Destination.ShouldBe("setback-room");
        climax.Exits[Direction.North].Destination.ShouldBe("treasure-room");

        var treasure = _adventure.World.GetRoom("treasure-room");
        treasure.Exits[Direction.South].Destination.ShouldBe("climax-room");
    }
}
