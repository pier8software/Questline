using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class VersionQueryHandlerTests
{
    [Fact]
    public void Returns_version_response()
    {
        var state = new GameBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .BuildState("player1", "tavern");

        var handler = new VersionQueryHandler();

        var result = handler.Handle(state, new Requests.VersionQuery());

        var version = result.ShouldBeOfType<Responses.VersionResponse>();
        version.Version.ShouldNotBeNullOrEmpty();
    }
}
