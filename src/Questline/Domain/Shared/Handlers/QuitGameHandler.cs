using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Messages;
using Questline.Framework.Mediator;

namespace Questline.Domain.Shared.Handlers;

public class QuitGameHandler : IRequestHandler<Requests.QuitGame>
{
    public IResponse Handle(GameState state, Requests.QuitGame request) => new Responses.GameQuited();
}
