using Questline.Domain.Shared.Data;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class DropItemCommandHandler : IRequestHandler<Requests.DropItemCommand>
{
    public IResponse Handle(GameState state, Requests.DropItemCommand command)
    {
        var item = state.Player.Character.FindInventoryItemByName(command.ItemName);

        if (item is null)
        {
            return new ErrorResponse($"You are not carrying '{command.ItemName}'.");
        }

        state.Player.Character.RemoveInventoryItem(item);

        var room = state.GetRoom(state.Player.Character.Location);
        room.AddItem(item);

        return new Responses.ItemDroppedResponse(item.Name);
    }
}
