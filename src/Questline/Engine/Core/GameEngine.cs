using Questline.Domain.Adventures.Entity;
using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Engine.Characters;
using Questline.Engine.Content;
using Questline.Engine.Messages;
using Questline.Engine.Parsers;
using Questline.Engine.Services;
using Questline.Framework.Mediator;

namespace Questline.Engine.Core;

public class GameEngine(
    Parser                        parser,
    RequestSender                 dispatcher,
    IGameContentLoader            contentLoader,
    PlaythroughService            playthroughService,
    CharacterCreationStateMachine stateMachine)
{
    private readonly Dictionary<int, Resources.AdventureSummary> _adventures = new()
    {
        [1] = new Resources.AdventureSummary("the-goblins-lair", "The Goblins Lair")
    };

    private GamePhase  _phase          = GamePhase.Login;
    private Player     _player         = null!;
    private string     _startingRoomId = "";
    private GameState? _state;


    public GamePhase Phase => _phase;


    public IResponse LoadWorld(string adventureId)
    {
        var world = contentLoader.Load(adventureId);
        _startingRoomId = world.StartingRoomId;
        _state          = new GameState(world.Rooms, barriers: world.Barriers, adventureId: adventureId);

        return stateMachine.ProcessInput(null);
    }

    public IResponse ProcessInput(string? input)
    {
        return _phase switch
        {
            GamePhase.Login              => HandleLogin(),
            GamePhase.AdventureSelection => HandleAdventureSelection(input),
            GamePhase.CharacterCreation  => HandleCharacterCreation(input),
            GamePhase.Playing            => HandleGamePlay(input),
            GamePhase.Ended              => new Responses.GameQuitResponse(),
            _                            => throw new InvalidOperationException($"Unexpected game phase: {_phase}")
        };
    }

    private IResponse HandleLogin()
    {
        _player = Player.Create("Rich");
        _phase  = GamePhase.AdventureSelection;
        return new Responses.LoginResponse(_player.Name);
    }

    private IResponse HandleAdventureSelection(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new Responses.GetAdventuresResponse(_adventures.Values.ToArray());
        }

        if (int.TryParse(input.Trim(), out var id))
        {
            var adventureContent = contentLoader.Load(_adventures[id].Id);
            Adventure.Create();
            playthroughService.CreatePlaythrough(_player.Id, adventureContent);
        }
        else
        {
            return new ErrorResponse("Invalid selection");
        }

        _phase = GamePhase.CharacterCreation;
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

        var startingRoom   = _state.GetRoom(_startingRoomId);
        var exits          = startingRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        var items          = startingRoom.Items.Select(i => i.Name).ToList();
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
