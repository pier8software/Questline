using Questline.Domain;
using Questline.Engine.Commands;

namespace Questline.Engine.Handlers;

public class InventoryCommandHandler : ICommandHandler<InventoryCommand>
{
    public CommandResult Execute(GameState state, InventoryCommand command)
    {
        var items = state.Player.Inventory.Items
            .Select(i => i.Name)
            .ToList();

        return new InventoryResult(items);
    }
}
