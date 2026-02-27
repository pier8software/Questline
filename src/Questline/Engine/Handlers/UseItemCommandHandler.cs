using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Engine.Handlers;

public class UseItemCommandHandler(
    IGameSession           session,
    IPlaythroughRepository playthroughRepository,
    IRoomRepository        roomRepository) : IRequestHandler<Requests.UseItemCommand>
{
    public async Task<IResponse> Handle(Requests.UseItemCommand command)
    {
        var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);
        var item        = playthrough.FindInventoryItemByName(command.ItemName);

        if (item is null)
        {
            return new ErrorResponse($"You don't have '{command.ItemName}'.");
        }

        var room = await roomRepository.GetById(playthrough.Location);

        Barrier? barrier = null;

        if (command.TargetName is not null)
        {
            foreach (var (_, exit) in room.Exits)
            {
                if (exit.Barrier is not null &&
                    exit.Barrier.Name.Equals(command.TargetName, StringComparison.OrdinalIgnoreCase))
                {
                    barrier = exit.Barrier;
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
            foreach (var (_, exit) in room.Exits)
            {
                if (exit.Barrier is not null && !playthrough.IsBarrierUnlocked(exit.Barrier.Id))
                {
                    barrier = exit.Barrier;
                    break;
                }
            }

            if (barrier is null)
            {
                return new ErrorResponse("There is nothing to use that on.");
            }
        }

        if (playthrough.IsBarrierUnlocked(barrier.Id))
        {
            return new ErrorResponse($"The {barrier.Name} is already unlocked.");
        }

        if (item.Id != barrier.UnlockItemId)
        {
            return new ErrorResponse($"The {item.Name} doesn't work on the {barrier.Name}.");
        }

        playthrough.UnlockBarrier(barrier.Id);
        await playthroughRepository.Save(playthrough);

        return new Responses.UseItemResponse(barrier.UnlockMessage);
    }
}
