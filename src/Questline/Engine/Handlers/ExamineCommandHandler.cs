using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class ExamineCommandHandler(
    IGameSession           session,
    IPlaythroughRepository playthroughRepository,
    IRoomRepository        roomRepository) : IRequestHandler<Requests.ExamineCommand>
{
    public async Task<IResponse> Handle(Requests.ExamineCommand command)
    {
        var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);

        // Search order: inventory items > room items > room features
        var inventoryItem = playthrough.FindInventoryItemByName(command.TargetName);
        if (inventoryItem is not null)
        {
            return new Responses.ExamineResponse(inventoryItem.Description);
        }

        var room      = await roomRepository.GetById(playthrough.AdventureId, playthrough.Location);
        var roomItems = playthrough.GetRecordedRoomItems(room.Id) ?? room.Items.ToList();

        var roomItem = roomItems.FirstOrDefault(i =>
            i.Name.Equals(command.TargetName, StringComparison.OrdinalIgnoreCase));
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
