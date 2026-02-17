using Questline.Domain.Players.Entity;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Handlers;
using Questline.Domain.Shared.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Domain.Shared.Handlers;

public class QuitGameHandlerTests
{
    [Fact]
    public void Returns_quited_response()
    {
        var world = new GameBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .Build();

        var state = new GameState(world, new Player { Id = "player1", Location = "tavern" });

        var handler = new QuitGameHandler();

        var result = handler.Handle(state, new Requests.QuitGame());

        result.ShouldBeOfType<Responses.GameQuited>();
        result.Message.ShouldBe("Goodbye!");
    }
}
