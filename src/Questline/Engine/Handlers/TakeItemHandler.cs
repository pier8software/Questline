using Questline.Domain.Shared.Data;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class TakeItemHandler : IRequestHandler<Requests.TakeItemCommand>
{
    public IResponse Handle(GameState state, Requests.TakeItemCommand request)
    {
        var room = state.GetRoom(state.Player.Character.Location);
        var item = room.FindItemByName(request.ItemName);

        if (item is null)
        {
            return new ErrorResponse($"There is no '{request.ItemName}' here.");
        }

        room.RemoveItem(item);
        state.Player.Character.AddInventoryItem(item);

        return new Responses.ItemTakenResponse(item.Name);
    }
}
