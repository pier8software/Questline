using Questline.Domain.Playthroughs.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class MovePlayerCommandHandler(
    IGameSession           session,
    IPlaythroughRepository playthroughRepository,
    IRoomRepository        roomRepository) : IRequestHandler<Requests.MovePlayerCommand>
{
    public async Task<IResponse> Handle(Requests.MovePlayerCommand command)
    {
        var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);
        var currentRoom = await roomRepository.GetById(playthrough.AdventureId, playthrough.Location);

        if (!currentRoom.Exits.TryGetValue(command.Direction, out var exit))
        {
            return new ErrorResponse($"There is no exit to the {command.Direction}.");
        }

        if (exit.Barrier is not null && !playthrough.IsBarrierUnlocked(exit.Barrier.Id))
        {
            return new ErrorResponse(exit.Barrier.BlockedMessage);
        }

        playthrough.MoveTo(exit.Destination);
        await playthroughRepository.Save(playthrough);

        var newRoom        = await roomRepository.GetById(playthrough.AdventureId, exit.Destination);
        var roomItems      = playthrough.GetRecordedRoomItems(newRoom.Id) ?? newRoom.Items.ToList();
        var exits          = newRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        var items          = roomItems.Select(i => i.Name).ToList();
        var lockedBarriers = GetLockedBarrierDescriptions(newRoom.Exits, playthrough);

        return new Responses.PlayerMovedResponse(newRoom.Name, newRoom.Description, exits, items, lockedBarriers);
    }

    private static List<string> GetLockedBarrierDescriptions(
        IReadOnlyDictionary<Direction, Exit> exits,
        Playthrough                          playthrough)
    {
        var descriptions = new List<string>();
        foreach (var (_, exit) in exits)
        {
            if (exit.Barrier is not null && !playthrough.IsBarrierUnlocked(exit.Barrier.Id))
            {
                descriptions.Add(exit.Barrier.Description);
            }
        }

        return descriptions;
    }
}
