using Questline.Domain.Playthroughs.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class GetRoomDetailsHandler(
    IGameSession           session,
    IPlaythroughRepository playthroughRepository,
    IRoomRepository        roomRepository) : IRequestHandler<Requests.GetRoomDetailsQuery>
{
    public async Task<IResponse> Handle(Requests.GetRoomDetailsQuery request)
    {
        var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);
        var room        = await roomRepository.GetById(playthrough.AdventureId, playthrough.Location);

        var roomItems      = playthrough.GetRecordedRoomItems(room.Id) ?? room.Items.ToList();
        var exits          = room.Exits.Keys.Select(d => d.ToString()).ToList();
        var items          = roomItems.Select(i => i.Name).ToList();
        var lockedBarriers = GetLockedBarrierDescriptions(room.Exits, playthrough);

        return new Responses.RoomDetailsResponse(room.Name, room.Description, exits, items, lockedBarriers);
    }

    private static List<string> GetLockedBarrierDescriptions(
        IReadOnlyDictionary<Direction, Exit> exits,
        Playthrough                          playthrough)
    {
        var descriptions = new List<string>();
        foreach (var (_, exit) in exits)
        {
            if (exit.Barrier is not null && !playthrough.IsBarrierUnlocked(exit.Barrier.Id))
            {
                descriptions.Add(exit.Barrier.Description);
            }
        }

        return descriptions;
    }
}
