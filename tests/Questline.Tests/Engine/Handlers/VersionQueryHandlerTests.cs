using Questline.Engine.Handlers;
using Questline.Engine.Messages;

namespace Questline.Tests.Engine.Handlers;

public class VersionQueryHandlerTests
{
    [Fact]
    public async Task Returns_version_response_with_current_version()
    {
        var handler = new VersionQueryHandler();

        var result = await handler.Handle(new Requests.VersionQuery());

        var versionResult = result.ShouldBeOfType<Responses.VersionResponse>();
        versionResult.Version.ShouldNotBeNullOrEmpty();
    }
}
