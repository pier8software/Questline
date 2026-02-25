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
    public void Returns_response_with_room_details()
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
            .WithRoom("entrance", "Entrance", "The entrance.")
            .BuildState("player1", "hallway");
        var handler = new GetRoomDetailsHandler();

        var result = handler.Handle(state, new Requests.GetRoomDetailsQuery());

        var details = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        details.RoomName.ShouldBe("Hallway");
        details.Description.ShouldBe("A long hallway.");
        details.Items.ShouldContain("brass lamp");
        details.Exits.ShouldContain("North");
        details.Exits.ShouldContain("South");
    }

    [Fact]
    public void Response_has_empty_items_if_room_is_empty()
    {
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .BuildState("player1", "cellar");
        var handler = new GetRoomDetailsHandler();

        var result = handler.Handle(state, new Requests.GetRoomDetailsQuery());

        var details = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        details.Items.ShouldBeEmpty();
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

        var details = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        details.LockedBarriers.ShouldContain("A heavy iron door blocks the way North.");
    }

    [Fact]
    public void Response_omits_barrier_when_unlocked()
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
        barrier.Unlock();

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.",
                r => r.WithExit(Direction.North, new Exit("beyond", "iron-door")))
            .WithRoom("beyond", "Beyond", "Beyond the door.")
            .WithBarrier(barrier)
            .BuildState("player1", "chamber");

        var handler = new GetRoomDetailsHandler();

        var result = handler.Handle(state, new Requests.GetRoomDetailsQuery());

        var details = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        details.LockedBarriers.ShouldBeEmpty();
    }
}
