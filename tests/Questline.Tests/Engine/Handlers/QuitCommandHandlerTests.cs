using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;

namespace Questline.Tests.Engine.Handlers;

public class QuitCommandHandlerTests
{
    [Fact]
    public void Execute_ReturnsQuitResult()
    {
        var world = new WorldBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "tavern" });
        var handler = new QuitCommandHandler();

        var result = handler.Execute(state, new QuitCommand());

        result.ShouldBeOfType<QuitResult>();
    }

    [Fact]
    public void Execute_ReturnsSuccessTrue()
    {
        var world = new WorldBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "tavern" });
        var handler = new QuitCommandHandler();

        var result = handler.Execute(state, new QuitCommand());

        result.Success.ShouldBeTrue();
    }

    [Fact]
    public void Execute_ReturnsGoodbyeMessage()
    {
        var world = new WorldBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "tavern" });
        var handler = new QuitCommandHandler();

        var result = handler.Execute(state, new QuitCommand());

        result.Message.ShouldBe("Goodbye!");
    }
}
