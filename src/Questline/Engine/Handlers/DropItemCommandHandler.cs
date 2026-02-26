using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class DropItemCommandHandler : IRequestHandler<Requests.DropItemCommand>
{
    public Task<IResponse> Handle(GameState state, Requests.DropItemCommand command)
    {
        var item = state.Character.FindInventoryItemByName(command.ItemName);

        if (item is null)
        {
            return Task.FromResult<IResponse>(new Responses.ItemDroppedResponse($"You are not carrying '{command.ItemName}'."));
        }

        state.Character.RemoveInventoryItem(item);

        var room = state.Adventure.GetRoom(state.Character.Location);
        room.AddItem(item);

        return Task.FromResult<IResponse>(new Responses.ItemDroppedResponse(item.Name));
    }
}
