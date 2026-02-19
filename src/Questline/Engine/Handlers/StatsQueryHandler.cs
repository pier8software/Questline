using Questline.Domain.Shared.Data;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class StatsQueryHandler : IRequestHandler<Requests.StatsQuery>
{
    public IResponse Handle(GameState state, Requests.StatsQuery request)
    {
        var character = state.Player.Character;
        return new Responses.StatsResponse(
            character.Name,
            character.Race.ToString(),
            character.Class.ToString(),
            character.Level,
            character.Stats!);
    }
}
