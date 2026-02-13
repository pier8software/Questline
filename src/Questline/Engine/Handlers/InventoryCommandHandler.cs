using Questline.Domain;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class InventoryCommandHandler : ICommandHandler<Commands.InventoryCommand>
{
    public CommandResult Execute(GameState state, Commands.InventoryCommand command)
    {
        var items = state.Player.Inventory.Items
            .Select(i => i.Name)
            .ToList();

        return new Results.InventoryResult(items);
    }
}
