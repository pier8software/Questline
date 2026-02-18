using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Engine.Handlers;

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

    [Fact]
    public void Locked_barrier_blocks_movement()
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

        result.Message.ShouldBe("The iron door is locked tight.");
        state.Player.Location.ShouldBe("start");
    }

    [Fact]
    public void Unlocked_barrier_allows_movement()
    {
        var barrier = new Barrier
        {
            Id = "iron-door",
            Name = "iron door",
            Description = "A heavy iron door.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId = "rusty-key",
            UnlockMessage = "The rusty key turns in the lock...",
            IsUnlocked = true
        };

        var state = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.",
                r => r.WithExit(Direction.North, new Exit("end", "iron-door")))
            .WithRoom("end", "End Room", "The end room.", r => r.WithExit(Direction.South, "start"))
            .WithBarrier(barrier)
            .BuildState("player1", "start");

        var handler = new MovePlayerCommandHandler();

        var result = handler.Handle(state, new Requests.MovePlayerCommand(Direction.North));

        var parts = result.Message.Split("\n");
        parts[0].ShouldBe("End Room");
        state.Player.Location.ShouldBe("end");
    }

    [Fact]
    public void Exit_without_barrier_allows_movement()
    {
        var state = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end", "End Room", "The end room.", r => r.WithExit(Direction.South, "start"))
            .BuildState("player1", "start");

        var handler = new MovePlayerCommandHandler();

        var result = handler.Handle(state, new Requests.MovePlayerCommand(Direction.North));

        var parts = result.Message.Split("\n");
        parts[0].ShouldBe("End Room");
        state.Player.Location.ShouldBe("end");
    }
}
