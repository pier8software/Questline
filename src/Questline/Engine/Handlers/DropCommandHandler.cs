using Questline.Domain;
using Questline.Engine.Commands;

namespace Questline.Engine.Handlers;

public class DropCommandHandler : ICommandHandler<DropCommand>
{
    public CommandResult Execute(GameState state, DropCommand command)
    {
        var item = state.Player.Inventory.FindByName(command.ItemName);

        if (item is null)
        {
            return new ErrorResult($"You are not carrying '{command.ItemName}'.");
        }

        state.Player.Inventory.Remove(item);
        var room = state.World.GetRoom(state.Player.Location);
        room.Items.Add(item);

        return new ItemDroppedResult(item.Name);
    }
}
