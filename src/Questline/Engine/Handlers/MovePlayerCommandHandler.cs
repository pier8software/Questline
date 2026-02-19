using Questline.Domain.Shared.Data;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class MovePlayerCommandHandler : IRequestHandler<Requests.MovePlayerCommand>
{
    public IResponse Handle(GameState state, Requests.MovePlayerCommand command)
    {
        var currentRoom = state.GetRoom(state.Player.Character.Location);

        if (!currentRoom.Exits.TryGetValue(command.Direction, out var exit))
        {
            return Responses.PlayerMovedResponse.Error($"There is no exit to the {command.Direction}.");
        }

        if (exit.BarrierId is not null)
        {
            var barrier = state.GetBarrier(exit.BarrierId);
            if (barrier is not null && !barrier.IsUnlocked)
            {
                return Responses.PlayerMovedResponse.Error(barrier.BlockedMessage);
            }
        }

        state.Player.Character.Location = exit.Destination;

        var newRoom = state.GetRoom(exit.Destination);
        var exits = newRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = newRoom.Items.Items.Select(i => i.Name).ToList();
        var lockedBarriers = GetLockedBarrierDescriptions(state, newRoom);

        return Responses.PlayerMovedResponse.Success(newRoom.Name, newRoom.Description, exits, items, lockedBarriers);
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

            var b = state.GetBarrier(roomExit.BarrierId);
            if (b is not null && !b.IsUnlocked)
            {
                descriptions.Add(b.Description);
            }
        }

        return descriptions;
    }
}
