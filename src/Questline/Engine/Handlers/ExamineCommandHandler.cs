using Questline.Domain.Shared.Data;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class ExamineCommandHandler : IRequestHandler<Requests.ExamineCommand>
{
    public IResponse Handle(GameState state, Requests.ExamineCommand command)
    {
        // Search order: inventory items > room items > room features
        var inventoryItem = state.Player.Character.FindInventoryItemByName(command.TargetName);
        if (inventoryItem is not null)
        {
            return new Responses.ExamineResponse(inventoryItem.Description);
        }

        var room = state.GetRoom(state.Player.Character.Location);

        var roomItem = room.FindItemByName(command.TargetName);
        if (roomItem is not null)
        {
            return new Responses.ExamineResponse(roomItem.Description);
        }

        var feature = room.Features.FirstOrDefault(f =>
            f.Name.Equals(command.TargetName, StringComparison.OrdinalIgnoreCase) ||
            f.Keywords.Any(k => k.Equals(command.TargetName, StringComparison.OrdinalIgnoreCase)));
        if (feature is not null)
        {
            return new Responses.ExamineResponse(feature.Description);
        }

        return new ErrorResponse($"You don't see '{command.TargetName}' here.");
    }
}
