using Questline.Domain;
using Questline.Engine.Commands;

namespace Questline.Engine.Handlers;

public class GetCommandHandler : ICommandHandler<GetCommand>
{
    public CommandResult Execute(GameState state, GetCommand command)
    {
        var room = state.World.GetRoom(state.Player.Location);
        var item = room.Items.FindByName(command.ItemName);

        if (item is null)
        {
            return new ErrorResult($"There is no '{command.ItemName}' here.");
        }

        room.Items.Remove(item);
        state.Player.Inventory.Add(item);

        return new ItemPickedUpResult(item.Name);
    }
}
