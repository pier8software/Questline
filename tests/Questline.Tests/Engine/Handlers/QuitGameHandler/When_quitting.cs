using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Tests.Engine.Handlers.QuitGameHandler;

public class When_quitting
{
    private readonly Questline.Engine.Handlers.QuitGameHandler _handler = new();

    [Fact]
    public async Task Returns_quit_response()
    {
        var result = await _handler.Handle(new PartyActor(), new Requests.QuitGame());

        result.ShouldBeOfType<Responses.GameQuitedResponse>();
    }
}
