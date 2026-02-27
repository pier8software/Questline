using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class TakeItemHandler(
    IGameSession           session,
    IPlaythroughRepository playthroughRepository,
    IRoomRepository        roomRepository) : IRequestHandler<Requests.TakeItemCommand>
{
    public async Task<IResponse> Handle(Requests.TakeItemCommand request)
    {
        var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);
        var room        = await roomRepository.GetById(playthrough.AdventureId, playthrough.Location);

        var roomItems = playthrough.GetRecordedRoomItems(room.Id) ?? room.Items.ToList();
        var item      = roomItems.FirstOrDefault(i => i.Name.Equals(request.ItemName, StringComparison.OrdinalIgnoreCase));

        if (item is null)
        {
            return new ErrorResponse($"There is no '{request.ItemName}' here.");
        }

        roomItems.Remove(item);
        playthrough.RecordRoomItems(room.Id, roomItems);
        playthrough.AddInventoryItem(item);
        await playthroughRepository.Save(playthrough);

        return new Responses.ItemTakenResponse(item.Name);
    }
}
