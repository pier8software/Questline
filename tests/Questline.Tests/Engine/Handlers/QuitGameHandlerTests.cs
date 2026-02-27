using Questline.Engine.Handlers;
using Questline.Engine.Messages;

namespace Questline.Tests.Engine.Handlers;

public class QuitGameHandlerTests
{
    [Fact]
    public async Task Returns_quited_response()
    {
        var handler = new QuitGameHandler();

        var result = await handler.Handle(new Requests.QuitGame());

        result.ShouldBeOfType<Responses.GameQuitedResponse>();
    }
}
