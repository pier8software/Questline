using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class GetPlayerInventoryQueryHandler(
    IGameSession           session,
    IPlaythroughRepository playthroughRepository) : IRequestHandler<Requests.GetPlayerInventoryQuery>
{
    public async Task<IResponse> Handle(Requests.GetPlayerInventoryQuery command)
    {
        var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);

        var items = playthrough.Inventory
            .Select(i => i.Name)
            .ToList();

        return new Responses.PlayerInventoryResponse(items);
    }
}
