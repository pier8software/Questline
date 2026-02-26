using Questline.Domain.Players.Entity;
using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class LoginCommandHandler : IRequestHandler<Requests.LoginCommand>
{
    public Task<IResponse> Handle(GameState state, Requests.LoginCommand request) =>
        Task.FromResult<IResponse>(new Responses.LoggedInResponse(Player.Create(Guid.NewGuid().ToString(), request.Username, "Rich"), []));
}
