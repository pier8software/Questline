using Questline.Domain.Shared.Data;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class GetPlayerInventoryQueryHandler : IRequestHandler<Requests.GetPlayerInventoryQuery>
{
    public IResponse Handle(GameState state, Requests.GetPlayerInventoryQuery command)
    {
        var items = state.Player.Character.Inventory.Items
            .Select(i => i.Name)
            .ToList();

        return new Responses.PlayerInventoryResponse(items);
    }
}
