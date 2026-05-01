using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Tests.Engine.Handlers.VersionQueryHandler;

public class When_querying_version
{
    private readonly Questline.Engine.Handlers.VersionQueryHandler _handler = new();

    [Fact]
    public async Task Returns_version_response_with_current_version()
    {
        var result = await _handler.Handle(new NoActor(), new Requests.VersionQuery());

        var versionResult = result.ShouldBeOfType<Responses.VersionResponse>();
        versionResult.Version.ShouldNotBeNullOrEmpty();
    }
}
