using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Engine.Content;
using Questline.Engine.Messages;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;
using Questline.Framework.Persistence;

namespace Questline.Engine.Core;

public class GameEngine(Parser parser, RequestSender dispatcher, IGameContentLoader contentLoader, IGameStateRepository repository)
{
    private GameState? _state;
    private string _startingRoomId = "";

    public void LoadWorld(string adventureId)
    {
        var world = contentLoader.Load(adventureId);
        _startingRoomId = world.StartingRoomId;
        _state = new GameState(world.Rooms, barriers: world.Barriers, adventureId: adventureId);
        repository.Save(_state);
    }

    public IResponse StartGame(Character character)
    {
        character.MoveTo(_startingRoomId);
        _state!.SetPlayer(new Player(Guid.NewGuid().ToString(), character));
        repository.Save(_state);

        var startingRoom = _state.GetRoom(_startingRoomId);
        var exits = startingRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = startingRoom.Items.Select(i => i.Name).ToList();
        var lockedBarriers = GetLockedBarrierDescriptions(startingRoom.Exits);

        return Responses.GameInitialisedResponse.Create(
            character.Name,
            startingRoom.Name,
            startingRoom.Description,
            exits,
            items,
            lockedBarriers);
    }

    public IResponse ProcessInput(string? input)
    {
        var parseResult = parser.Parse(input);
        if (!parseResult.IsSuccess)
        {
            return parseResult.Error!;
        }

        return dispatcher.Send(_state, parseResult.Request!);
    }

    private List<string> GetLockedBarrierDescriptions(IReadOnlyDictionary<Direction, Exit> roomExits)
    {
        var descriptions = new List<string>();
        foreach (var (_, exit) in roomExits)
        {
            if (exit.BarrierId is null)
            {
                continue;
            }

            var barrier = _state!.GetBarrier(exit.BarrierId);
            if (barrier is not null && !barrier.IsUnlocked)
            {
                descriptions.Add(barrier.Description);
            }
        }

        return descriptions;
    }
}
