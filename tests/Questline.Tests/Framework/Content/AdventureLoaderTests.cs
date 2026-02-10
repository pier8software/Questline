using System.Text.Json;
using Questline.Domain;
using Questline.Framework.Content;

namespace Questline.Tests.Framework.Content;

public class AdventureLoaderTests : IDisposable
{
    private readonly string _tempDir;
    private readonly FileSystemAdventureLoader _loader = new();

    public AdventureLoaderTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "questline-tests-" + Guid.NewGuid());
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
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
    public void Load_ValidAdventure_ConstructsWorld()
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
        room1.Exits[Direction.North].ShouldBe("room-2");

        var room2 = adventure.World.GetRoom("room-2");
        room2.Name.ShouldBe("Second Room");
        room2.Exits.ShouldContainKey(Direction.South);
    }

    [Fact]
    public void Load_ItemsPlacedInCorrectRooms()
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
    public void Load_ExitObjectWithBarrier_ExtractsDestination()
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
        roomA.Exits[Direction.North].ShouldBe("room-b");
    }

    [Fact]
    public void Load_MissingAdventureJson_Throws()
    {
        WriteRoomsJson("""{ "rooms": [] }""");
        WriteItemsJson("""{ "items": [] }""");

        var ex = Should.Throw<FileNotFoundException>(() => _loader.Load(_tempDir));
        ex.Message.ShouldContain("adventure.json");
    }

    [Fact]
    public void Load_InvalidJson_Throws()
    {
        WriteAdventureJson();
        WriteRoomsJson("{ not valid json }}}");
        WriteItemsJson("""{ "items": [] }""");

        Should.Throw<JsonException>(() => _loader.Load(_tempDir));
    }

    [Fact]
    public void Load_ExitToNonexistentRoom_Throws()
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
    public void Load_RoomReferencesUnknownItem_Throws()
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
    public void Load_StartingRoomMissing_Throws()
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
    public void Load_OrphanedRoom_Throws()
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
    public void Load_InvalidDirection_Throws()
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
