using System.Collections.Immutable;
using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Engine.Content;
using Questline.Engine.Messages;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;

namespace Questline.Engine.Core;

public class GameEngine(Parser parser, RequestSender dispatcher, IGameContentLoader contentLoader)
{
    private GameState? _state;

    public IResponse ProcessInput(string? input)
    {
        var parseResult = parser.Parse(input);
        if (!parseResult.IsSuccess)
        {
            return parseResult.Error!;
        }

        return dispatcher.Send(_state, parseResult.Request!);
    }

    public IResponse LaunchGame(Character character, string adventureId)
    {
        var world = contentLoader.Load(adventureId);

        character = character.MoveTo(world.StartingRoomId);

        _state = new GameState(world.Rooms, new Player(Guid.NewGuid().ToString(), character), world.Barriers);
        var startingRoom = _state.GetRoom(world.StartingRoomId);
        var exits = startingRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = startingRoom.Items.Items.Select(i => i.Name).ToList();
        var lockedBarriers = GetLockedBarrierDescriptions(startingRoom.Exits);

        List<string> GetLockedBarrierDescriptions(ImmutableDictionary<Direction, Exit> roomExits)
        {
            var descriptions = new List<string>();
            foreach (var (_, exit) in roomExits)
            {
                if (exit.BarrierId is null)
                {
                    continue;
                }

                var barrier = _state.GetBarrier(exit.BarrierId);
                if (barrier is not null && !barrier.IsUnlocked)
                {
                    descriptions.Add(barrier.Description);
                }
            }

            return descriptions;
        }

        return Responses.GameInitialisedResponse.Create(
            character.Name,
            startingRoom.Name,
            startingRoom.Description,
            exits,
            items,
            lockedBarriers);
    }
}
