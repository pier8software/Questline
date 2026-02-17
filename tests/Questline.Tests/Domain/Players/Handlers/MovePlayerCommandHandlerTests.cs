using Questline.Domain.Players.Entity;
using Questline.Domain.Players.Handlers;
using Questline.Domain.Players.Messages;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Domain.Players.Handlers;

public class MovePlayerCommandHandlerTests
{
    [Fact]
    public void Returns_next_room_details_in_response()
    {
        var rooms = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end", "End Room", "The end room.", r => r.WithExit(Direction.South, "start"))
            .Build();

        var player = new Player { Id = "player1", Location = "start" };
        var state = new GameState(rooms, player);

        var handler = new MovePlayerCommandHandler();

        var result = handler.Handle(state, new Requests.MovePlayerCommand(Direction.North));

        var parts = result.Message.Split("\n");
        parts[0].ShouldBe("End Room");
        parts[1].ShouldBe("The end room.");
    }

    [Fact]
    public void Invalid_direction_returns_error_message()
    {
        var rooms = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end", "End Room", "The end room.", r => r.WithExit(Direction.South, "start"))
            .Build();

        var player = new Player { Id = "player1", Location = "start" };
        var state = new GameState(rooms, player);

        var handler = new MovePlayerCommandHandler();

        var result = handler.Handle(state, new Requests.MovePlayerCommand(Direction.East));

        result.Message.ShouldBe("There is no exit to the East.");
    }

    [Fact]
    public void Player_location_is_updated_after_successful_move()
    {
        var rooms = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end", "End Room", "The end room.", r => r.WithExit(Direction.South, "start"))
            .Build();

        var player = new Player { Id = "player1", Location = "start" };
        var state = new GameState(rooms, player);

        var handler = new MovePlayerCommandHandler();

        _ = handler.Handle(state, new Requests.MovePlayerCommand(Direction.North));

        state.Player.Location.ShouldBe("end");
    }

    [Fact]
    public void Player_location_is_not_updated_if_move_is_not_possible()
    {
        var world = new GameBuilder()
            .WithRoom("sealed", "Sealed Room", "No way north.")
            .Build();

        var player = new Player { Id = "player1", Location = "sealed" };
        var state = new GameState(world, player);

        var handler = new MovePlayerCommandHandler();

        _ = handler.Handle(state, new Requests.MovePlayerCommand(Direction.North));

        player.Location.ShouldBe("sealed");
    }
}
