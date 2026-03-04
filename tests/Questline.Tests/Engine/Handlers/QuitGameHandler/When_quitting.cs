using Questline.Engine.Handlers;
using Questline.Engine.Messages;

namespace Questline.Tests.Engine.Handlers;

public class When_quitting
{
    private readonly QuitGameHandler _handler = new();

    [Fact]
    public async Task Returns_quit_response()
    {
        var result = await _handler.Handle(new Requests.QuitGame());

        result.ShouldBeOfType<Responses.GameQuitedResponse>();
    }
}
