using Questline.Domain.Messages;
using Questline.Domain.Players.Entity;
using Questline.Domain.Players.Handlers;
using Questline.Domain.Players.Messages;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Domain.Players.Handlers;

public class MovePlayerHandlerTests
{
    [Fact]
    public void Player_moves_to_destination_when_exit_exists()
    {
        var world = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end", "End Room", "The end room.", r => r.WithExit(Direction.South, "start"))
            .Build();
        var player = new Player { Id = "player1", Location = "start" };
        var state = new GameState(world, player);
        var handler = new MovePlayerHandler();

        var result = handler.Execute(state, new Questline.Domain.Players.Messages.Commands.MovePlayer(Direction.North));

        player.Location.ShouldBe("end");
        var moved = result.ShouldBeOfType<Events.PlayerMoved>();
        moved.RoomName.ShouldBe("End Room");
        moved.Description.ShouldBe("The end room.");
    }

    [Fact]
    public void Player_stays_put_when_no_exit_exists()
    {
        var world = new GameBuilder()
            .WithRoom("sealed", "Sealed Room", "No way north.")
            .Build();
        var player = new Player { Id = "player1", Location = "sealed" };
        var state = new GameState(world, player);
        var handler = new MovePlayerHandler();

        var result = handler.Execute(state, new Questline.Domain.Players.Messages.Commands.MovePlayer(Direction.North));

        player.Location.ShouldBe("sealed");
        result.ShouldBeOfType<Results.CommandError>();
        result.Success.ShouldBeFalse();
    }

    [Fact]
    public void Result_includes_exits_of_destination_room()
    {
        var world = new GameBuilder()
            .WithRoom("a", "Room A", "First room.", r => r.WithExit(Direction.East, "b"))
            .WithRoom("b", "Room B", "Second room.", r =>
            {
                r.WithExit(Direction.West, "a");
                r.WithExit(Direction.North, "c");
            })
            .WithRoom("c", "Room C", "Third room.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "a" });
        var handler = new MovePlayerHandler();

        var result = handler.Execute(state, new Questline.Domain.Players.Messages.Commands.MovePlayer(Direction.East));

        var moved = result.ShouldBeOfType<Events.PlayerMoved>();
        moved.Exits.ShouldContain("West");
        moved.Exits.ShouldContain("North");
    }

    [Fact]
    public void Result_includes_items_in_destination_room()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new GameBuilder()
            .WithRoom("a", "Room A", "First room.", r => r.WithExit(Direction.North, "b"))
            .WithRoom("b", "Room B", "Second room.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "a" });
        var handler = new MovePlayerHandler();

        var result = handler.Execute(state, new Questline.Domain.Players.Messages.Commands.MovePlayer(Direction.North));

        var moved = result.ShouldBeOfType<Events.PlayerMoved>();
        moved.Items.ShouldContain("brass lamp");
        moved.Message.ShouldContain("You can see");
    }
}
