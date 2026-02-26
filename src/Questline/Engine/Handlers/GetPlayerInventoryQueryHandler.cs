using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class GetPlayerInventoryQueryHandler : IRequestHandler<Requests.GetPlayerInventoryQuery>
{
    public Task<IResponse> Handle(GameState state, Requests.GetPlayerInventoryQuery command)
    {
        var items = state.Character.Inventory
            .Select(i => i.Name)
            .ToList();

        return Task.FromResult<IResponse>(new Responses.PlayerInventoryResponse(items));
    }
}
