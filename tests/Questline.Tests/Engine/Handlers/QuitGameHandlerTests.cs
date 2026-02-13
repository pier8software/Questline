using Questline.Domain.Entities;
using Questline.Domain.Shared;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class QuitGameHandlerTests
{
    [Fact]
    public void Returns_successful_quit_result()
    {
        var world = new GameBuilder()
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
        var world = new GameBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "tavern" });
        var handler = new QuitGameHandler();

        var result = handler.Execute(state, new Commands.QuitGame());

        result.Message.ShouldBe("Goodbye!");
    }
}
