using Questline.Domain.Messages;
using Questline.Domain.Players.Messages;
using Questline.Domain.Shared.Data;
using Questline.Framework.Mediator;
using Commands = Questline.Domain.Players.Messages.Commands;

namespace Questline.Domain.Players.Handlers;

public class DropPlayerItemHandler : ICommandHandler<Commands.DropPlayerItem>
{
    public CommandResult Execute(GameState state, Messages.Commands.DropPlayerItem command)
    {
        var item = state.Player.Inventory.FindByName(command.ItemName);

        if (item is null)
        {
            return new Results.CommandError($"You are not carrying '{command.ItemName}'.");
        }

        state.Player.Inventory.Remove(item);
        var room = state.GetRoom(state.Player.Location);
        room.Items.Add(item);

        return new Events.PlayerItemDropped(item.Name);
    }
}
