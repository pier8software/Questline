using Questline.Domain.Messages;
using Questline.Domain.Rooms.Messages;
using Questline.Domain.Shared;
using Questline.Domain.Shared.Data;
using Questline.Framework.Mediator;
using Commands = Questline.Domain.Messages.Commands;

namespace Questline.Domain.Rooms.Handlers;

public class TakeRoomItemHandler : ICommandHandler<Messages.Commands.TakeRoomItem>
{
    public CommandResult Execute(GameState state, Messages.Commands.TakeRoomItem command)
    {
        var room = state.GetRoom(state.Player.Location);
        var item = room.Items.FindByName(command.ItemName);

        if (item is null)
        {
            return new Results.CommandError($"There is no '{command.ItemName}' here.");
        }

        room.Items.Remove(item);
        state.Player.Inventory.Add(item);

        return new Events.RoomItemTaken(item.Name);
    }
}
