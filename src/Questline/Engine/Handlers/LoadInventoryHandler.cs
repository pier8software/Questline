using Questline.Domain;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class LoadInventoryHandler : ICommandHandler<Commands.LoadInventory>
{
    public CommandResult Execute(GameState state, Commands.LoadInventory command)
    {
        var items = state.Player.Inventory.Items
            .Select(i => i.Name)
            .ToList();

        return new Results.InventoryLoaded(items);
    }
}
