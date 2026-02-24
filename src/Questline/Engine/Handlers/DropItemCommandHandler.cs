using Questline.Domain.Shared.Data;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class DropItemCommandHandler : IRequestHandler<Requests.DropItemCommand>
{
    public IResponse Handle(GameState state, Requests.DropItemCommand command)
    {
        var item = state.Player.Character.Inventory.FindByName(command.ItemName);

        if (item is null)
        {
            return new Responses.ItemDroppedResponse($"You are not carrying '{command.ItemName}'.");
        }

        var newCharacter = state.Player.Character.RemoveInventoryItem(item);
        state.UpdatePlayer(state.Player with { Character = newCharacter });

        var room = state.GetRoom(state.Player.Character.Location);
        state.UpdateRoom(room.AddItem(item));

        return new Responses.ItemDroppedResponse(item.Name);
    }
}
