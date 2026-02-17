using Questline.Domain.Rooms.Messages;
using Questline.Domain.Shared.Data;
using Questline.Framework.Mediator;

namespace Questline.Domain.Rooms.Handlers;

public class GetRoomDetailsHandler : IRequestHandler<Requests.GetRoomDetailsQuery>
{
    public IResponse Handle(GameState state, Requests.GetRoomDetailsQuery request)
    {
        var room = state.GetRoom(state.Player.Location);
        var exits = room.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = room.Items.Items.Select(i => i.Name).ToList();

        return Responses.RoomDetailsResponse.Success(room.Name, room.Description, exits, items);
    }
}
