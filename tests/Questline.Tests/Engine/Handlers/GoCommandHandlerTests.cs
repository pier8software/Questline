using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;

namespace Questline.Tests.Engine.Handlers;

public class GoCommandHandlerTests
{
    [Fact]
    public void GoNorth_WhenExitExists_MovesPlayerToDestination()
    {
        var world = new WorldBuilder()
            .WithRoom("start", "Start", "Starting room.", r => r.WithExit(Direction.North, "end"))
            .WithRoom("end", "End Room", "The end room.", r => r.WithExit(Direction.South, "start"))
            .Build();
        var player = new Player { Id = "player1", Location = "start" };
        var state = new GameState(world, player);
        var handler = new GoCommandHandler();

        var result = handler.Execute(state, new GoCommand(Direction.North));

        player.Location.ShouldBe("end");
        var moved = result.ShouldBeOfType<MovedResult>();
        moved.RoomName.ShouldBe("End Room");
        moved.Description.ShouldBe("The end room.");
    }

    [Fact]
    public void GoNorth_WhenNoExit_ReturnsErrorAndPlayerUnmoved()
    {
        var world = new WorldBuilder()
            .WithRoom("sealed", "Sealed Room", "No way north.")
            .Build();
        var player = new Player { Id = "player1", Location = "sealed" };
        var state = new GameState(world, player);
        var handler = new GoCommandHandler();

        var result = handler.Execute(state, new GoCommand(Direction.North));

        player.Location.ShouldBe("sealed");
        result.ShouldBeOfType<ErrorResult>();
        result.Success.ShouldBeFalse();
    }

    [Fact]
    public void Go_WhenExitExists_ReturnsExitsOfNewRoom()
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

        var result = handler.Execute(state, new GoCommand(Direction.East));

        var moved = result.ShouldBeOfType<MovedResult>();
        moved.Exits.ShouldContain("West");
        moved.Exits.ShouldContain("North");
    }

    [Fact]
    public void Go_WhenDestinationHasItems_IncludesThemInResult()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("a", "Room A", "First room.", r => r.WithExit(Direction.North, "b"))
            .WithRoom("b", "Room B", "Second room.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "a" });
        var handler = new GoCommandHandler();

        var result = handler.Execute(state, new GoCommand(Direction.North));

        var moved = result.ShouldBeOfType<MovedResult>();
        moved.Items.ShouldContain("brass lamp");
        moved.Message.ShouldContain("You can see");
    }
}
