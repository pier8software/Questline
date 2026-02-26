using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class MovePlayerCommandHandler : IRequestHandler<Requests.MovePlayerCommand>
{
    public Task<IResponse> Handle(GameState state, Requests.MovePlayerCommand command)
    {
        var currentRoom = state.Adventure.GetRoom(state.Character.Location);

        if (!currentRoom.Exits.TryGetValue(command.Direction, out var exit))
        {
            return Task.FromResult<IResponse>(new ErrorResponse($"There is no exit to the {command.Direction}."));
        }

        if (exit.BarrierId is not null)
        {
            var barrier = state.Adventure.GetBarrier(exit.BarrierId);
            if (barrier is not null && !barrier.IsUnlocked)
            {
                return Task.FromResult<IResponse>(new ErrorResponse(barrier.BlockedMessage));
            }
        }

        state.Character.MoveTo(exit.Destination);

        var newRoom        = state.Adventure.GetRoom(exit.Destination);
        var exits          = newRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        var items          = newRoom.Items.Select(i => i.Name).ToList();
        var lockedBarriers = GetLockedBarrierDescriptions(state, newRoom);

        return Task.FromResult<IResponse>(new Responses.PlayerMovedResponse(newRoom.Name, newRoom.Description, exits, items, lockedBarriers));
    }

    private static List<string> GetLockedBarrierDescriptions(GameState state, Domain.Rooms.Entity.Room room)
    {
        var descriptions = new List<string>();
        foreach (var (_, roomExit) in room.Exits)
        {
            if (roomExit.BarrierId is null)
            {
                continue;
            }

            var b = state.Adventure.GetBarrier(roomExit.BarrierId);
            if (b is not null && !b.IsUnlocked)
            {
                descriptions.Add(b.Description);
            }
        }

        return descriptions;
    }
}
