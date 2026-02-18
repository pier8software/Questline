using Questline.Domain.Shared.Data;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class QuitGameHandler : IRequestHandler<Requests.QuitGame>
{
    public IResponse Handle(GameState state, Requests.QuitGame request) => new Responses.GameQuited();
}
