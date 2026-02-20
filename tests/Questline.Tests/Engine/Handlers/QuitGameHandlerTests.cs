using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class QuitGameHandlerTests
{
    [Fact]
    public void Returns_quited_response()
    {
        var state = new GameBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .BuildState("player1", "tavern");

        var handler = new QuitGameHandler();

        var result = handler.Handle(state, new Requests.QuitGame());

        result.ShouldBeOfType<Responses.GameQuited>();
        result.Message.ShouldBe("Goodbye!");
    }
}
