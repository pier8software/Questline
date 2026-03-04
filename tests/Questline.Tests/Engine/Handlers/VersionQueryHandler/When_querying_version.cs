using Questline.Engine.Handlers;
using Questline.Engine.Messages;

namespace Questline.Tests.Engine.Handlers;

public class When_querying_version
{
    private readonly VersionQueryHandler _handler = new();

    [Fact]
    public async Task Returns_version_response_with_current_version()
    {
        var result = await _handler.Handle(new Requests.VersionQuery());

        var versionResult = result.ShouldBeOfType<Responses.VersionResponse>();
        versionResult.Version.ShouldNotBeNullOrEmpty();
    }
}
