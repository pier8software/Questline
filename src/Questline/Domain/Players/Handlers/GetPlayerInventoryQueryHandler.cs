using Questline.Domain.Players.Messages;
using Questline.Domain.Shared.Data;
using Questline.Framework.Mediator;

namespace Questline.Domain.Players.Handlers;

public class GetPlayerInventoryQueryHandler : IRequestHandler<Requests.GetPlayerInventoryQuery>
{
    public IResponse Handle(GameState state, Requests.GetPlayerInventoryQuery command)
    {
        var items = state.Player.Inventory.Items
            .Select(i => i.Name)
            .ToList();

        return new Responses.PlayerInventoryResponse(items);
    }
}
