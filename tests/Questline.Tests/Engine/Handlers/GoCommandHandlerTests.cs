using Questline.Domain;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;

namespace Questline.Tests.Engine.Handlers;

public class GoCommandHandlerTests
{
    [Fact]
    public void Player_moves_to_destination_when_exit_exists()
    {
        var world = new WorldBuilder()
            .WithRoom("start", "Start", "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end", "End Room", "The end room.", r => r.WithExit(Direction.South, "start"))
            .Build();
        var player = new Player { Id = "player1", Location = "start" };
        var state = new GameState(world, player);
        var handler = new GoCommandHandler();

        var result = handler.Execute(state, new Commands.GoCommand(Direction.North));

        player.Location.ShouldBe("end");
        var moved = result.ShouldBeOfType<Results.MovedResult>();
        moved.RoomName.ShouldBe("End Room");
        moved.Description.ShouldBe("The end room.");
    }

    [Fact]
    public void Player_stays_put_when_no_exit_exists()
    {
        var world = new WorldBuilder()
            .WithRoom("sealed", "Sealed Room", "No way north.")
            .Build();
        var player = new Player { Id = "player1", Location = "sealed" };
        var state = new GameState(world, player);
        var handler = new GoCommandHandler();

        var result = handler.Execute(state, new Commands.GoCommand(Direction.North));

        player.Location.ShouldBe("sealed");
        result.ShouldBeOfType<Results.ErrorResult>();
        result.Success.ShouldBeFalse();
    }

    [Fact]
    public void Result_includes_exits_of_destination_room()
    {
        var world = new WorldBuilder()
            .WithRoom("a", "Room A", "First room.", r => r.WithExit(Direction.East, "b"))
            .WithRoom("b", "Room B", "Second room.", r =>
            {
                r.WithExit(Direction.West, "a");
                r.WithExit(Direction.North, "c");
            })
            .WithRoom("c", "Room C", "Third room.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "a" });
        var handler = new GoCommandHandler();

        var result = handler.Execute(state, new Commands.GoCommand(Direction.East));

        var moved = result.ShouldBeOfType<Results.MovedResult>();
        moved.Exits.ShouldContain("West");
        moved.Exits.ShouldContain("North");
    }

    [Fact]
    public void Result_includes_items_in_destination_room()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("a", "Room A", "First room.", r => r.WithExit(Direction.North, "b"))
            .WithRoom("b", "Room B", "Second room.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "a" });
        var handler = new GoCommandHandler();

        var result = handler.Execute(state, new Commands.GoCommand(Direction.North));

        var moved = result.ShouldBeOfType<Results.MovedResult>();
        moved.Items.ShouldContain("brass lamp");
        moved.Message.ShouldContain("You can see");
    }
}
