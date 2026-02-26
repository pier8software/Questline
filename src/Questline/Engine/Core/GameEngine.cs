using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Engine.Characters;
using Questline.Engine.Content;
using Questline.Engine.Messages;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;

namespace Questline.Engine.Core;

public class GameState
{
    public GamePhase        Phase     { get; set; } = GamePhase.Started;
    public Player           Player    { get; set; } = null!;
    public AdventureContent Adventure { get; set; } = null!;
    public Character        Character { get; set; } = null!;
}

public enum GamePhase
{
    Started,
    Login,
    NewAdventure,
    CharacterCreation,
    Playing,
    Ended
}

public class GameEngine(
    Parser                        parser,
    RequestSender                 dispatcher,
    IGameContentLoader            contentLoader,
    CharacterCreationStateMachine stateMachine)
{
    private readonly GameState _state = new();

    private readonly IReadOnlyDictionary<int, string> _adventures = new Dictionary<int, string>
    {
        [1] = "the-goblins-lair"
    };

    public GamePhase Phase => _state.Phase;


    public async Task<IResponse> ProcessInput(string? input)
    {
        switch (_state.Phase)
        {
            case GamePhase.Started:
                return HandleGameStarted();
            case GamePhase.Login:
                return await HandleLogin(input);
            case GamePhase.NewAdventure:
                return HandleNewAdventure(input);
            case GamePhase.CharacterCreation:
                return HandleCharacterCreation(input);
            case GamePhase.Playing:
                return await HandleGamePlay(input);
            case GamePhase.Ended:
                return new Responses.GameQuitedResponse();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<IResponse> HandleGamePlay(string? input)
    {
        var parseResult = parser.Parse(input);
        if (!parseResult.IsSuccess)
        {
            return parseResult.Error!;
        }

        var response = await dispatcher.Send(_state, parseResult.Request!);

        if (response is Responses.GameQuitedResponse)
        {
            _state.Phase = GamePhase.Ended;
        }

        return response;
    }

    private IResponse HandleCharacterCreation(string? input)
    {
        var response = stateMachine.ProcessInput(input);

        if (stateMachine.CompletedCharacter is { } character)
        {
            _state.Character = character;
            _state.Phase     = GamePhase.Playing;
            return StartAdventure(character);
        }

        return response;
    }

    private IResponse HandleNewAdventure(string? input)
    {
        if (!int.TryParse(input, out var adventureId) || !_adventures.ContainsKey(adventureId))
        {
            return new ErrorResponse("Invalid selection.");
        }

        _state.Adventure = contentLoader.Load(_adventures[adventureId]);
        _state.Phase     = GamePhase.CharacterCreation;

        return new Responses.NewAdventureSelectedResponse();
    }

    private async Task<IResponse> HandleLogin(string? input)
    {
        var parseResult = parser.Parse(input);
        if (!parseResult.IsSuccess)
        {
            return parseResult.Error!;
        }

        var response         = await dispatcher.Send(_state, parseResult.Request!);
        var loggedInResponse = response as Responses.LoggedInResponse;

        _state.Player = loggedInResponse!.Player;
        _state.Phase  = GamePhase.NewAdventure;

        return loggedInResponse with
        {
            Adventures = [new Resources.AdventureSummary("the-goblins-lair", "The Goblins' Lair")]
        };
    }

    private IResponse HandleGameStarted()
    {
        _state.Phase = GamePhase.Login;
        return new Responses.GameStartedResponse();
    }

    private IResponse StartAdventure(Character character)
    {
        character.MoveTo(_state.Adventure.StartingRoomId);

        var startingRoom   = _state.Adventure.GetRoom(_state.Adventure.StartingRoomId);
        var exits          = startingRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        var items          = startingRoom.Items.Select(i => i.Name).ToList();
        var lockedBarriers = GetLockedBarrierDescriptions(startingRoom.Exits);

        return new Responses.AdventureStartedResponse(
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

            var barrier = _state.Adventure.GetBarrier(exit.BarrierId);
            if (barrier is not null && !barrier.IsUnlocked)
            {
                descriptions.Add(barrier.Description);
            }
        }

        return descriptions;
    }
}
