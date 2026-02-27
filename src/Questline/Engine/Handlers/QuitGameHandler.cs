using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class QuitGameHandler : IRequestHandler<Requests.QuitGame>
{
    public Task<IResponse> Handle(Requests.QuitGame request) =>
        Task.FromResult<IResponse>(new Responses.GameQuitedResponse());
}
