using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class StatsQueryHandler(
    IGameSession           session,
    IPlaythroughRepository playthroughRepository) : IRequestHandler<Requests.StatsQuery>
{
    public async Task<IResponse> Handle(Actor actor, Requests.StatsQuery request)
    {
        var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);
        return new Responses.StatsResponse(playthrough.ToPartySummary());
    }
}
