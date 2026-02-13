using Questline.Domain;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class DropItemHandler : ICommandHandler<Commands.DropItem>
{
    public CommandResult Execute(GameState state, Commands.DropItem command)
    {
        var item = state.Player.Inventory.FindByName(command.ItemName);

        if (item is null)
        {
            return new Results.CommandError($"You are not carrying '{command.ItemName}'.");
        }

        state.Player.Inventory.Remove(item);
        var room = state.World.GetRoom(state.Player.Location);
        room.Items.Add(item);

        return new Results.ItemDropped(item.Name);
    }
}
