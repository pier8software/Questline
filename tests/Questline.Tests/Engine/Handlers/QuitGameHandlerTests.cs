using Questline.Domain;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;

namespace Questline.Tests.Engine.Handlers;

public class QuitGameHandlerTests
{
    [Fact]
    public void Returns_successful_quit_result()
    {
        var world = new WorldBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "tavern" });
        var handler = new QuitGameHandler();

        var result = handler.Execute(state, new Commands.QuitGame());

        result.ShouldBeOfType<Results.GameQuited>();
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public void Displays_goodbye_message()
    {
        var world = new WorldBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "tavern" });
        var handler = new QuitGameHandler();

        var result = handler.Execute(state, new Commands.QuitGame());

        result.Message.ShouldBe("Goodbye!");
    }
}
