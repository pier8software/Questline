using Questline.Domain.Shared.Data;
using Questline.Framework.Mediator;
using Requests = Questline.Domain.Players.Messages.Requests;
using Responses = Questline.Domain.Players.Messages.Responses;

namespace Questline.Domain.Players.Handlers;

public class DropItemCommandHandler : IRequestHandler<Requests.DropItemCommand, Responses.ItemDroppedResponse>
{
    public Responses.ItemDroppedResponse Handle(GameState state, Requests.DropItemCommand command)
    {
        var item = state.Player.Inventory.FindByName(command.ItemName);

        if (item is null)
        {
            return new Responses.ItemDroppedResponse($"You are not carrying '{command.ItemName}'.");
        }

        state.Player.Inventory.Remove(item);
        var room = state.GetRoom(state.Player.Location);
        room.Items.Add(item);

        return new Responses.ItemDroppedResponse(item.Name);
    }
}
