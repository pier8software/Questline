using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Engine.Handlers;

public class GetRoomDetailsHandlerTests
{
    [Fact]
    public async Task Returns_response_with_room_details()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var state = new GameBuilder()
            .WithRoom("hallway", "Hallway", "A long hallway.", r =>
            {
                r.WithItem(lamp);
                r.WithExit(Direction.North, "throne-room");
                r.WithExit(Direction.South, "entrance");
            })
            .WithRoom("throne-room", "Throne Room", "Grand throne room.")
            .WithRoom("entrance",    "Entrance",    "The entrance.")
            .BuildState("player1", "hallway");
        var handler = new GetRoomDetailsHandler();

        var result = await handler.Handle(state, new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.RoomName.ShouldBe("Hallway");
        lookResult.Description.ShouldBe("A long hallway.");
        lookResult.Items.ShouldContain("brass lamp");
        lookResult.Exits.ShouldContain("North");
        lookResult.Exits.ShouldContain("South");
    }

    [Fact]
    public async Task Response_omits_items_when_room_is_empty()
    {
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .BuildState("player1", "cellar");
        var handler = new GetRoomDetailsHandler();

        var result = await handler.Handle(state, new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.Items.ShouldBeEmpty();
    }

    [Fact]
    public async Task Response_includes_locked_barrier_description()
    {
        var barrier = new Barrier
        {
            Id             = "iron-door",
            Name           = "iron door",
            Description    = "A heavy iron door blocks the way North.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId   = "rusty-key",
            UnlockMessage  = "The rusty key turns in the lock..."
        };

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.",
                r => r.WithExit(Direction.North, new Exit("beyond", "iron-door")))
            .WithRoom("beyond", "Beyond", "Beyond the door.")
            .WithBarrier(barrier)
            .BuildState("player1", "chamber");

        var handler = new GetRoomDetailsHandler();

        var result = await handler.Handle(state, new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.LockedBarriers.ShouldContain("A heavy iron door blocks the way North.");
    }

    [Fact]
    public async Task Response_omits_barrier_line_when_unlocked()
    {
        var barrier = new Barrier
        {
            Id             = "iron-door",
            Name           = "iron door",
            Description    = "A heavy iron door blocks the way North.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId   = "rusty-key",
            UnlockMessage  = "The rusty key turns in the lock..."
        };
        barrier.Unlock();

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.",
                r => r.WithExit(Direction.North, new Exit("beyond", "iron-door")))
            .WithRoom("beyond", "Beyond", "Beyond the door.")
            .WithBarrier(barrier)
            .BuildState("player1", "chamber");

        var handler = new GetRoomDetailsHandler();

        var result = await handler.Handle(state, new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.LockedBarriers.ShouldBeEmpty();
    }
}
