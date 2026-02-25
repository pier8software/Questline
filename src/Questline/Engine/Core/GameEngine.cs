using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Engine.Characters;
using Questline.Engine.Content;
using Questline.Engine.Messages;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;

namespace Questline.Engine.Core;

public class GameEngine(
    Parser parser,
    RequestSender dispatcher,
    IGameContentLoader contentLoader,
    CharacterCreationStateMachine stateMachine)
{
    private GamePhase _phase = GamePhase.CharacterCreation;
    private string _startingRoomId = "";
    private GameState? _state;

    public GamePhase Phase => _phase;

    public IResponse LoadWorld(string adventureId)
    {
        var world = contentLoader.Load(adventureId);
        _startingRoomId = world.StartingRoomId;
        _state = new GameState(world.Rooms, barriers: world.Barriers, adventureId: adventureId);

        return stateMachine.ProcessInput(null);
    }

    public IResponse ProcessInput(string? input)
    {
        return _phase switch
        {
            GamePhase.CharacterCreation => HandleCharacterCreation(input),
            GamePhase.Playing => HandleGamePlay(input),
            GamePhase.Ended => new Responses.GameQuitResponse(),
            _ => throw new InvalidOperationException($"Unexpected game phase: {_phase}")
        };
    }

    private IResponse HandleCharacterCreation(string? input)
    {
        var response = stateMachine.ProcessInput(input);

        if (stateMachine.CompletedCharacter is { } character)
        {
            _phase = GamePhase.Playing;
            return StartAdventure(character);
        }

        return response;
    }

    private IResponse HandleGamePlay(string? input)
    {
        var parseResult = parser.Parse(input);
        if (!parseResult.IsSuccess)
        {
            return parseResult.Error!;
        }

        var response = dispatcher.Send(_state, parseResult.Request!);

        if (response is Responses.GameQuitResponse)
        {
            _phase = GamePhase.Ended;
        }

        return response;
    }

    private IResponse StartAdventure(Character character)
    {
        character.MoveTo(_startingRoomId);
        _state!.SetPlayer(new Player(Guid.NewGuid().ToString(), character));

        var startingRoom = _state.GetRoom(_startingRoomId);
        var exits = startingRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = startingRoom.Items.Select(i => i.Name).ToList();
        var lockedBarriers = GetLockedBarrierDescriptions(startingRoom.Exits);

        return new Responses.GameStartedResponse(
            character.ToSummary(),
            startingRoom.Name,
            startingRoom.Description,
            exits,
            items,
            lockedBarriers);
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
