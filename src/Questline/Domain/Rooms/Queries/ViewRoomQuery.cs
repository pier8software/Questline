using Questline.Domain.Rooms.Messages;
using Questline.Domain.Shared;
using Questline.Domain.Shared.Data;
using Questline.Framework.Mediator;
using Commands = Questline.Domain.Messages.Commands;

namespace Questline.Domain.Rooms.Queries;

public class ViewRoomQuery : ICommandHandler<Messages.Commands.ViewRoom>
{
    public CommandResult Execute(GameState state, Messages.Commands.ViewRoom command)
    {
        var room = state.GetRoom(state.Player.Location);
        var exits = room.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = room.Items.Items.Select(i => i.Name).ToList();
        return new Events.RoomViewed(room.Name, room.Description, exits, items);
    }
}
