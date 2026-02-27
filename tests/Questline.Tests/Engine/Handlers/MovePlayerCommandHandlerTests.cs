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
    public async Task Returns_next_room_details_in_response()
    {
        var fixture = new GameBuilder()
            .WithRoom("start", "Start",    "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end",   "End Room", "The end room.",  r => r.WithExit(Direction.South, "start"))
            .Build("start");

        var handler = new MovePlayerCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.MovePlayerCommand(Direction.North));

        var moveResult = result.ShouldBeOfType<Responses.PlayerMovedResponse>();
        moveResult.RoomName.ShouldBe("End Room");
        moveResult.Description.ShouldBe("The end room.");
    }

    [Fact]
    public async Task Invalid_direction_returns_error_message()
    {
        var fixture = new GameBuilder()
            .WithRoom("start", "Start",    "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end",   "End Room", "The end room.",  r => r.WithExit(Direction.South, "start"))
            .Build("start");

        var handler = new MovePlayerCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.MovePlayerCommand(Direction.East));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("There is no exit to the East.");
    }

    [Fact]
    public async Task Player_location_is_updated_after_moving()
    {
        var fixture = new GameBuilder()
            .WithRoom("start", "Start",    "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end",   "End Room", "The end room.",  r => r.WithExit(Direction.South, "start"))
            .Build("start");

        var handler = new MovePlayerCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        _ = await handler.Handle(new Requests.MovePlayerCommand(Direction.North));

        fixture.Playthrough.Location.ShouldBe("end");
    }

    [Fact]
    public async Task Player_location_is_not_updated_if_direction_is_invalid()
    {
        var fixture = new GameBuilder()
            .WithRoom("sealed", "Sealed Room", "No way north.")
            .Build("sealed");

        var handler = new MovePlayerCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        _ = await handler.Handle(new Requests.MovePlayerCommand(Direction.North));

        fixture.Playthrough.Location.ShouldBe("sealed");
    }

    [Fact]
    public async Task Player_location_is_not_updated_if_exit_is_blocked()
    {
        var barrier = new Barrier
        {
            Id             = "iron-door",
            Name           = "iron door",
            Description    = "A heavy iron door.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId   = "rusty-key",
            UnlockMessage  = "The rusty key turns in the lock..."
        };

        var fixture = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.",
                r => r.WithExit(Direction.North, new Exit("end", barrier)))
            .WithRoom("end", "End Room", "The end room.")
            .Build("start");

        var handler = new MovePlayerCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.MovePlayerCommand(Direction.North));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("The iron door is locked tight.");
        fixture.Playthrough.Location.ShouldBe("start");
    }

    [Fact]
    public async Task Player_location_is_updated_when_barrier_is_unlocked()
    {
        var barrier = new Barrier
        {
            Id             = "iron-door",
            Name           = "iron door",
            Description    = "A heavy iron door.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId   = "rusty-key",
            UnlockMessage  = "The rusty key turns in the lock..."
        };

        var fixture = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.",
                r => r.WithExit(Direction.North, new Exit("end", barrier)))
            .WithRoom("end", "End Room", "The end room.", r => r.WithExit(Direction.South, "start"))
            .WithUnlockedBarrier("iron-door")
            .Build("start");

        var handler = new MovePlayerCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.MovePlayerCommand(Direction.North));

        var moveResult = result.ShouldBeOfType<Responses.PlayerMovedResponse>();
        moveResult.RoomName.ShouldBe("End Room");
        fixture.Playthrough.Location.ShouldBe("end");
    }
}
