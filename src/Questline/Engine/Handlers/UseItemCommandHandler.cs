using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Engine.Handlers;

public class UseItemCommandHandler : IRequestHandler<Requests.UseItemCommand>
{
    public IResponse Handle(GameState state, Requests.UseItemCommand command)
    {
        var item = state.Character.FindInventoryItemByName(command.ItemName);
        if (item is null)
        {
            return new ErrorResponse($"You don't have '{command.ItemName}'.");
        }

        var room = state.Adventure.GetRoom(state.Character.Location);

        Barrier? barrier = null;

        if (command.TargetName is not null)
        {
            // Targeted use: find barrier by name in room exits
            foreach (var (_, exit) in room.Exits)
            {
                if (exit.BarrierId is null)
                {
                    continue;
                }

                var b = state.Adventure.GetBarrier(exit.BarrierId);
                if (b is not null && b.Name.Equals(command.TargetName, StringComparison.OrdinalIgnoreCase))
                {
                    barrier = b;
                    break;
                }
            }

            if (barrier is null)
            {
                return new ErrorResponse($"You don't see '{command.TargetName}' here.");
            }
        }
        else
        {
            // Contextual use: find first locked barrier in room exits
            foreach (var (_, exit) in room.Exits)
            {
                if (exit.BarrierId is null)
                {
                    continue;
                }

                var b = state.Adventure.GetBarrier(exit.BarrierId);
                if (b is not null && !b.IsUnlocked)
                {
                    barrier = b;
                    break;
                }
            }

            if (barrier is null)
            {
                return new ErrorResponse("There is nothing to use that on.");
            }
        }

        if (barrier.IsUnlocked)
        {
            return new ErrorResponse($"The {barrier.Name} is already unlocked.");
        }

        if (item.Id != barrier.UnlockItemId)
        {
            return new ErrorResponse($"The {item.Name} doesn't work on the {barrier.Name}.");
        }

        barrier.Unlock();
        return new Responses.UseItemResponse(barrier.UnlockMessage);
    }
}
