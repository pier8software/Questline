using Questline.Domain;
using Questline.Domain.Shared;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class TakeItemHandler : ICommandHandler<Commands.TakeItem>
{
    public CommandResult Execute(GameState state, Commands.TakeItem command)
    {
        var room = state.GetRoom(state.Player.Location);
        var item = room.Items.FindByName(command.ItemName);

        if (item is null)
        {
            return new Results.CommandError($"There is no '{command.ItemName}' here.");
        }

        room.Items.Remove(item);
        state.Player.Inventory.Add(item);

        return new Results.ItemTaken(item.Name);
    }
}
