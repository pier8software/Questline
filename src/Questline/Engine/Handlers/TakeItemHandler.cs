using Questline.Domain.Shared.Data;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class TakeItemHandler : IRequestHandler<Requests.TakeItemCommand>
{
    public IResponse Handle(GameState state, Requests.TakeItemCommand request)
    {
        var room = state.GetRoom(state.Player.Character.Location);
        var item = room.Items.FindByName(request.ItemName);

        if (item is null)
        {
            return Responses.ItemTakenResponse.Error($"There is no '{request.ItemName}' here.");
        }

        state.UpdateRoom(room.RemoveItem(item));
        var newCharacter = state.Player.Character.AddInventoryItem(item);
        state.UpdatePlayer(state.Player with { Character = newCharacter });

        return Responses.ItemTakenResponse.Success(item.Name);
    }
}
