using Questline.Domain;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class GetCommandHandler : ICommandHandler<Commands.GetCommand>
{
    public CommandResult Execute(GameState state, Commands.GetCommand command)
    {
        var room = state.World.GetRoom(state.Player.Location);
        var item = room.Items.FindByName(command.ItemName);

        if (item is null)
        {
            return new Results.ErrorResult($"There is no '{command.ItemName}' here.");
        }

        room.Items.Remove(item);
        state.Player.Inventory.Add(item);

        return new Results.ItemPickedUpResult(item.Name);
    }
}
