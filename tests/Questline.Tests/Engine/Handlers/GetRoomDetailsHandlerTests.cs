using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Engine.Handlers;

public class GetRoomDetailsHandlerTests
{
    [Fact]
    public void Returns_response_with_formatted_message()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var rooms = new GameBuilder()
            .WithRoom("hallway", "Hallway", "A long hallway.", r =>
            {
                r.WithItem(lamp);
                r.WithExit(Direction.North, "throne-room");
                r.WithExit(Direction.South, "entrance");
            })
            .WithRoom("throne-room", "Throne Room", "Grand throne room.")
            .WithRoom("entrance", "Entrance", "The entrance.")
            .Build();

        var state = new GameState(rooms, new Player { Id = "player1", Location = "hallway" });
        var handler = new GetRoomDetailsHandler();

        var result = handler.Handle(state, new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        var parts = lookResult.Message.Split('\n');
        parts[0].ShouldBe("Hallway");
        parts[1].ShouldBe("A long hallway.");
        parts[2].ShouldContain("You can see: brass lamp");
        parts[3].ShouldContain("Exits: North, South");
    }

    [Fact]
    public void Response_omits_items_line_if_room_is_empty()
    {
        var world = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new GetRoomDetailsHandler();

        var result = handler.Handle(state, new Requests.GetRoomDetailsQuery());

        result.Message.ShouldNotContain("You can see");
    }

    [Fact]
    public void Response_includes_locked_barrier_description()
    {
        var barrier = new Barrier
        {
            Id = "iron-door",
            Name = "iron door",
            Description = "A heavy iron door blocks the way North.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId = "rusty-key",
            UnlockMessage = "The rusty key turns in the lock..."
        };

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.",
                r => r.WithExit(Direction.North, new Exit("beyond", "iron-door")))
            .WithRoom("beyond", "Beyond", "Beyond the door.")
            .WithBarrier(barrier)
            .BuildState("player1", "chamber");

        var handler = new GetRoomDetailsHandler();

        var result = handler.Handle(state, new Requests.GetRoomDetailsQuery());

        result.Message.ShouldContain("A heavy iron door blocks the way North.");
    }

    [Fact]
    public void Response_omits_barrier_line_when_unlocked()
    {
        var barrier = new Barrier
        {
            Id = "iron-door",
            Name = "iron door",
            Description = "A heavy iron door blocks the way North.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId = "rusty-key",
            UnlockMessage = "The rusty key turns in the lock...",
            IsUnlocked = true
        };

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.",
                r => r.WithExit(Direction.North, new Exit("beyond", "iron-door")))
            .WithRoom("beyond", "Beyond", "Beyond the door.")
            .WithBarrier(barrier)
            .BuildState("player1", "chamber");

        var handler = new GetRoomDetailsHandler();

        var result = handler.Handle(state, new Requests.GetRoomDetailsQuery());

        result.Message.ShouldNotContain("iron door");
    }
}
