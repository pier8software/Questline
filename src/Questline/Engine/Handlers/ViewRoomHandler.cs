using Questline.Domain;
using Questline.Domain.Shared;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class ViewRoomHandler : ICommandHandler<Commands.ViewRoom>
{
    public CommandResult Execute(GameState state, Commands.ViewRoom command)
    {
        var room = state.GetRoom(state.Player.Location);
        var exits = room.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = room.Items.Items.Select(i => i.Name).ToList();
        return new Results.RoomViewed(room.Name, room.Description, exits, items);
    }
}
