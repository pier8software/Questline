using Questline.Domain.Players.Entity;
using Questline.Domain.Shared.Data;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class LoginCommandHandler : IRequestHandler<Requests.LoginCommand>
{
    public IResponse Handle(GameState state, Requests.LoginCommand request) =>
        new Responses.LoggedInResponse(Player.Create(Guid.NewGuid().ToString(), request.Username, "Rich"), []);
}
