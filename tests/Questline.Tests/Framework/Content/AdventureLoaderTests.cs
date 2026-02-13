using System.Text.Json;
using Questline.Domain;
using Questline.Framework.Content;

namespace Questline.Tests.Framework.Content;

public class AdventureLoaderTests : IDisposable
{
    private readonly FileSystemAdventureLoader _loader = new();
    private readonly string _tempDir;

    public AdventureLoaderTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "questline-tests-" + Guid.NewGuid());
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    private void WriteAdventureJson(string id = "test", string name = "Test Adventure",
        string description = "A test.", string startingRoomId = "room-1", string version = "1.0")
    {
        File.WriteAllText(Path.Combine(_tempDir, "adventure.json"),
            $$"""
              {
                "id": "{{id}}",
                "name": "{{name}}",
                "description": "{{description}}",
                "startingRoomId": "{{startingRoomId}}",
                "version": "{{version}}"
              }
              """);
    }

    private void WriteRoomsJson(string json) =>
        File.WriteAllText(Path.Combine(_tempDir, "rooms.json"), json);

    private void WriteItemsJson(string json) =>
        File.WriteAllText(Path.Combine(_tempDir, "items.json"), json);

    private void WriteTwoRoomAdventure()
    {
        WriteAdventureJson(startingRoomId: "room-1");
        WriteRoomsJson("""
                       {
                         "rooms": [
                           {
                             "id": "room-1",
                             "name": "First Room",
                             "description": "The first room.",
                             "exits": { "north": "room-2" },
                             "items": ["sword"]
                           },
                           {
                             "id": "room-2",
                             "name": "Second Room",
                             "description": "The second room.",
                             "exits": { "south": "room-1" }
                           }
                         ]
                       }
                       """);
        WriteItemsJson("""
                       {
                         "items": [
                           { "id": "sword", "name": "iron sword", "description": "A sturdy blade." }
                         ]
                       }
                       """);
    }

    [Fact]
    public void Valid_adventure_constructs_world_with_rooms_and_exits()
    {
        WriteTwoRoomAdventure();

        var adventure = _loader.Load(_tempDir);

        adventure.Id.ShouldBe("test");
        adventure.Name.ShouldBe("Test Adventure");
        adventure.Description.ShouldBe("A test.");
        adventure.StartingRoomId.ShouldBe("room-1");

        var room1 = adventure.World.GetRoom("room-1");
        room1.Name.ShouldBe("First Room");
        room1.Exits.ShouldContainKey(Direction.North);
        room1.Exits[Direction.North].Destination.ShouldBe("room-2");

        var room2 = adventure.World.GetRoom("room-2");
        room2.Name.ShouldBe("Second Room");
        room2.Exits.ShouldContainKey(Direction.South);
    }

    [Fact]
    public void Items_are_placed_in_correct_rooms()
    {
        WriteTwoRoomAdventure();

        var adventure = _loader.Load(_tempDir);

        var room1 = adventure.World.GetRoom("room-1");
        room1.Items.Items.ShouldContain(i => i.Id == "sword");
        room1.Items.Items.ShouldContain(i => i.Name == "iron sword");

        var room2 = adventure.World.GetRoom("room-2");
        room2.Items.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void Exit_object_with_barrier_extracts_destination()
    {
        WriteAdventureJson(startingRoomId: "room-a");
        WriteRoomsJson("""
                       {
                         "rooms": [
                           {
                             "id": "room-a",
                             "name": "Room A",
                             "description": "First.",
                             "exits": {
                               "north": { "destination": "room-b", "barrier": "locked-door" }
                             }
                           },
                           {
                             "id": "room-b",
                             "name": "Room B",
                             "description": "Second.",
                             "exits": { "south": "room-a" }
                           }
                         ]
                       }
                       """);
        WriteItemsJson("""{ "items": [] }""");

        var adventure = _loader.Load(_tempDir);

        var roomA = adventure.World.GetRoom("room-a");
        roomA.Exits[Direction.North].Destination.ShouldBe("room-b");
        roomA.Exits[Direction.North].BarrierId.ShouldBe("locked-door");
    }

    [Fact]
    public void Missing_adventure_json_throws()
    {
        WriteRoomsJson("""{ "rooms": [] }""");
        WriteItemsJson("""{ "items": [] }""");

        var ex = Should.Throw<FileNotFoundException>(() => _loader.Load(_tempDir));
        ex.Message.ShouldContain("adventure.json");
    }

    [Fact]
    public void Invalid_json_throws()
    {
        WriteAdventureJson();
        WriteRoomsJson("{ not valid json }}}");
        WriteItemsJson("""{ "items": [] }""");

        Should.Throw<JsonException>(() => _loader.Load(_tempDir));
    }

    [Fact]
    public void Exit_to_nonexistent_room_throws()
    {
        WriteAdventureJson(startingRoomId: "room-1");
        WriteRoomsJson("""
                       {
                         "rooms": [
                           {
                             "id": "room-1",
                             "name": "Only Room",
                             "description": "Alone.",
                             "exits": { "north": "no-such-room" }
                           }
                         ]
                       }
                       """);
        WriteItemsJson("""{ "items": [] }""");

        var ex = Should.Throw<ContentValidationException>(() => _loader.Load(_tempDir));
        ex.Message.ShouldContain("no-such-room");
    }

    [Fact]
    public void Room_referencing_unknown_item_throws()
    {
        WriteAdventureJson(startingRoomId: "room-1");
        WriteRoomsJson("""
                       {
                         "rooms": [
                           {
                             "id": "room-1",
                             "name": "Room",
                             "description": "A room.",
                             "items": ["ghost-item"]
                           }
                         ]
                       }
                       """);
        WriteItemsJson("""{ "items": [] }""");

        var ex = Should.Throw<ContentValidationException>(() => _loader.Load(_tempDir));
        ex.Message.ShouldContain("ghost-item");
    }

    [Fact]
    public void Missing_starting_room_throws()
    {
        WriteAdventureJson(startingRoomId: "nonexistent");
        WriteRoomsJson("""
                       {
                         "rooms": [
                           {
                             "id": "room-1",
                             "name": "Room",
                             "description": "A room."
                           }
                         ]
                       }
                       """);
        WriteItemsJson("""{ "items": [] }""");

        var ex = Should.Throw<ContentValidationException>(() => _loader.Load(_tempDir));
        ex.Message.ShouldContain("nonexistent");
    }

    [Fact]
    public void Orphaned_room_throws()
    {
        WriteAdventureJson(startingRoomId: "room-1");
        WriteRoomsJson("""
                       {
                         "rooms": [
                           {
                             "id": "room-1",
                             "name": "Start",
                             "description": "Starting room."
                           },
                           {
                             "id": "orphan",
                             "name": "Orphaned Room",
                             "description": "Cannot be reached."
                           }
                         ]
                       }
                       """);
        WriteItemsJson("""{ "items": [] }""");

        var ex = Should.Throw<ContentValidationException>(() => _loader.Load(_tempDir));
        ex.Message.ShouldContain("orphan");
    }

    [Fact]
    public void Invalid_direction_throws()
    {
        WriteAdventureJson(startingRoomId: "room-1");
        WriteRoomsJson("""
                       {
                         "rooms": [
                           {
                             "id": "room-1",
                             "name": "Room",
                             "description": "A room.",
                             "exits": { "sideways": "room-1" }
                           }
                         ]
                       }
                       """);
        WriteItemsJson("""{ "items": [] }""");

        var ex = Should.Throw<ContentValidationException>(() => _loader.Load(_tempDir));
        ex.Message.ShouldContain("sideways");
    }
}
