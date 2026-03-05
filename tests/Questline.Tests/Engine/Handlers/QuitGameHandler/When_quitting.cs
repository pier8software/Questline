using Questline.Engine.Messages;

namespace Questline.Tests.Engine.Handlers.QuitGameHandler;

public class When_quitting
{
    private readonly Questline.Engine.Handlers.QuitGameHandler _handler = new();

    [Fact]
    public async Task Returns_quit_response()
    {
        var result = await _handler.Handle(new Requests.QuitGame());

        result.ShouldBeOfType<Responses.GameQuitedResponse>();
    }
}
