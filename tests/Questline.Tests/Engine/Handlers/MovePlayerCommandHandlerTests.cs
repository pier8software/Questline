using Questline.Domain.Rooms.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Engine.Handlers;

public class MovePlayerCommandHandlerTests
{
    [Fact]
    public void Returns_next_room_details_in_response()
    {
        var state = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end", "End Room", "The end room.", r => r.WithExit(Direction.South, "start"))
            .BuildState("player1", "start");

        var handler = new MovePlayerCommandHandler();

        var result = handler.Handle(state, new Requests.MovePlayerCommand(Direction.North));

        var moved = result.ShouldBeOfType<Responses.PlayerMovedResponse>();
        moved.RoomName.ShouldBe("End Room");
        moved.Description.ShouldBe("The end room.");
    }

    [Fact]
    public void Invalid_direction_returns_error_message()
    {
        var state = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end", "End Room", "The end room.", r => r.WithExit(Direction.South, "start"))
            .BuildState("player1", "start");

        var handler = new MovePlayerCommandHandler();

        var result = handler.Handle(state, new Requests.MovePlayerCommand(Direction.East));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("There is no exit to the East.");
    }

    [Fact]
    public void Player_location_is_updated_after_moving()
    {
        var state = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end", "End Room", "The end room.", r => r.WithExit(Direction.South, "start"))
            .BuildState("player1", "start");

        var handler = new MovePlayerCommandHandler();

        _ = handler.Handle(state, new Requests.MovePlayerCommand(Direction.North));

        state.Player.Character.Location.ShouldBe("end");
    }

    [Fact]
    public void Player_location_is_not_updated_if_direction_is_invalid()
    {
        var state = new GameBuilder()
            .WithRoom("sealed", "Sealed Room", "No way north.")
            .BuildState("player1", "sealed");

        var handler = new MovePlayerCommandHandler();

        _ = handler.Handle(state, new Requests.MovePlayerCommand(Direction.North));

        state.Player.Character.Location.ShouldBe("sealed");
    }

    [Fact]
    public void Player_location_is_not_updated_if_exit_is_blocked()
    {
        var barrier = new Barrier
        {
            Id = "iron-door",
            Name = "iron door",
            Description = "A heavy iron door.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId = "rusty-key",
            UnlockMessage = "The rusty key turns in the lock..."
        };

        var state = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.",
                r => r.WithExit(Direction.North, new Exit("end", "iron-door")))
            .WithRoom("end", "End Room", "The end room.")
            .WithBarrier(barrier)
            .BuildState("player1", "start");

        var handler = new MovePlayerCommandHandler();

        var result = handler.Handle(state, new Requests.MovePlayerCommand(Direction.North));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("The iron door is locked tight.");
        state.Player.Character.Location.ShouldBe("start");
    }

    [Fact]
    public void Player_location_is_updated_when_barrier_is_unlocked()
    {
        var barrier = new Barrier
        {
            Id = "iron-door",
            Name = "iron door",
            Description = "A heavy iron door.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId = "rusty-key",
            UnlockMessage = "The rusty key turns in the lock..."
        };
        barrier.Unlock();

        var state = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.",
                r => r.WithExit(Direction.North, new Exit("end", "iron-door")))
            .WithRoom("end", "End Room", "The end room.", r => r.WithExit(Direction.South, "start"))
            .WithBarrier(barrier)
            .BuildState("player1", "start");

        var handler = new MovePlayerCommandHandler();

        var result = handler.Handle(state, new Requests.MovePlayerCommand(Direction.North));

        var moved = result.ShouldBeOfType<Responses.PlayerMovedResponse>();
        moved.RoomName.ShouldBe("End Room");
        state.Player.Character.Location.ShouldBe("end");
    }
}
