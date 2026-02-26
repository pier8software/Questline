using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class GetRoomDetailsHandler : IRequestHandler<Requests.GetRoomDetailsQuery>
{
    public IResponse Handle(GameState state, Requests.GetRoomDetailsQuery request)
    {
        var room           = state.Adventure.GetRoom(state.Character.Location);
        var exits          = room.Exits.Keys.Select(d => d.ToString()).ToList();
        var items          = room.Items.Select(i => i.Name).ToList();
        var lockedBarriers = GetLockedBarrierDescriptions(state, room);

        return new Responses.RoomDetailsResponse(room.Name, room.Description, exits, items, lockedBarriers);
    }

    private static List<string> GetLockedBarrierDescriptions(GameState state, Domain.Rooms.Entity.Room room)
    {
        var descriptions = new List<string>();
        foreach (var (_, exit) in room.Exits)
        {
            if (exit.BarrierId is null)
            {
                continue;
            }

            var barrier = state.Adventure.GetBarrier(exit.BarrierId);
            if (barrier is not null && !barrier.IsUnlocked)
            {
                descriptions.Add(barrier.Description);
            }
        }

        return descriptions;
    }
}
