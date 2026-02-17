using Questline.Domain.Rooms.Messages;
using Questline.Domain.Shared.Data;
using Questline.Framework.Mediator;

namespace Questline.Domain.Rooms.Handlers;

public class GetRoomDetailsHandler : IRequestHandler<Requests.GetRoomDetailsQuery, Responses.RoomDetailsResponse>
{
    public Responses.RoomDetailsResponse Handle(GameState state, Requests.GetRoomDetailsQuery request)
    {
        var room = state.GetRoom(state.Player.Location);
        var exits = room.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = room.Items.Items.Select(i => i.Name).ToList();

        return new Responses.RoomDetailsResponse(room.Name, room.Description, exits, items);
    }
}
