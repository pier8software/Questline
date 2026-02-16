using Questline.Domain.Players.Messages;
using Questline.Domain.Shared.Data;
using Questline.Framework.Mediator;

namespace Questline.Domain.Players.Queries;

public class LoadPlayerInventoryQuery : ICommandHandler<Commands.LoadPlayerInventory>
{
    public CommandResult Execute(GameState state, Commands.LoadPlayerInventory command)
    {
        var items = state.Player.Inventory.Items
            .Select(i => i.Name)
            .ToList();

        return new Events.PlayerInventoryLoaded(items);
    }
}
