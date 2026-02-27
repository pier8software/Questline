using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class DropItemCommandHandler(
    IGameSession           session,
    IPlaythroughRepository playthroughRepository,
    IRoomRepository        roomRepository) : IRequestHandler<Requests.DropItemCommand>
{
    public async Task<IResponse> Handle(Requests.DropItemCommand command)
    {
        var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);
        var item        = playthrough.FindInventoryItemByName(command.ItemName);

        if (item is null)
        {
            return new Responses.ItemDroppedResponse($"You are not carrying '{command.ItemName}'.");
        }

        playthrough.RemoveInventoryItem(item);

        var room      = await roomRepository.GetById(playthrough.Location);
        var roomItems = playthrough.GetRecordedRoomItems(room.Id) ?? room.Items.ToList();
        roomItems.Add(item);
        playthrough.RecordRoomItems(room.Id, roomItems);
        await playthroughRepository.Save(playthrough);

        return new Responses.ItemDroppedResponse(item.Name);
    }
}
