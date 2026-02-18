using Questline.Domain.Players.Entity;
using Questline.Domain.Shared.Data;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class VersionQueryHandlerTests
{
    [Fact]
    public void Returns_version_response_with_current_version()
    {
        var world = new GameBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .Build();

        var state = new GameState(world, new Player { Id = "player1", Location = "tavern" });

        var handler = new VersionQueryHandler();

        var result = handler.Handle(state, new Requests.VersionQuery());

        result.ShouldBeOfType<Responses.VersionResponse>();
        result.Message.ShouldStartWith("Questline v");
    }
}
