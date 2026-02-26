using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class TakeItemHandler : IRequestHandler<Requests.TakeItemCommand>
{
    public Task<IResponse> Handle(GameState state, Requests.TakeItemCommand request)
    {
        var room = state.Adventure.GetRoom(state.Character.Location);
        var item = room.FindItemByName(request.ItemName);

        if (item is null)
        {
            return Task.FromResult<IResponse>(new ErrorResponse($"There is no '{request.ItemName}' here."));
        }

        room.RemoveItem(item);
        state.Character.AddInventoryItem(item);

        return Task.FromResult<IResponse>(new Responses.ItemTakenResponse(item.Name));
    }
}
