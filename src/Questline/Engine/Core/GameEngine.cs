using Questline.Domain.Playthroughs.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Engine.Characters;
using Questline.Engine.Messages;
using Questline.Engine.Parsers;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;

namespace Questline.Engine.Core;

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
    IRoomRepository               roomRepository,
    IPlaythroughRepository        playthroughRepository,
    IGameSession                  gameSession,
    CharacterCreationStateMachine stateMachine)
{
    private GamePhase _phase               = GamePhase.Started;
    private string    _selectedAdventureId = null!;

    private readonly IReadOnlyDictionary<int, string> _adventures = new Dictionary<int, string>
    {
        [1] = "the-goblins-lair"
    };

    public GamePhase Phase => _phase;

    public async Task<IResponse> ProcessInput(string? input)
    {
        switch (_phase)
        {
            case GamePhase.Started:
                return HandleGameStarted();
            case GamePhase.Login:
                return await HandleLogin(input);
            case GamePhase.NewAdventure:
                return await HandleNewAdventure(input);
            case GamePhase.CharacterCreation:
                return await HandleCharacterCreation(input);
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

        var response = await dispatcher.Send(parseResult.Request!);

        if (response is Responses.GameQuitedResponse)
        {
            _phase = GamePhase.Ended;
        }

        return response;
    }

    private async Task<IResponse> HandleCharacterCreation(string? input)
    {
        var response = stateMachine.ProcessInput(input);

        if (stateMachine.CompletedCharacter is { } character)
        {
            var startingRoomId = await roomRepository.GetStartingRoomId(_selectedAdventureId);
            var playthrough    = Playthrough.Create(_selectedAdventureId, startingRoomId, character);

            await playthroughRepository.Save(playthrough);
            gameSession.SetPlaythroughId(playthrough.Id);

            _phase = GamePhase.Playing;
            return await StartAdventure(playthrough);
        }

        return response;
    }

    private Task<IResponse> HandleNewAdventure(string? input)
    {
        if (!int.TryParse(input, out var adventureIndex) || !_adventures.TryGetValue(adventureIndex, out var adventureId))
        {
            return Task.FromResult(new ErrorResponse("Invalid selection.") as IResponse);
        }

        _selectedAdventureId = adventureId;
        _phase               = GamePhase.CharacterCreation;

        return Task.FromResult(new Responses.NewAdventureSelectedResponse() as IResponse);
    }

    private async Task<IResponse> HandleLogin(string? input)
    {
        var parseResult = parser.Parse(input);
        if (!parseResult.IsSuccess)
        {
            return parseResult.Error!;
        }

        var response         = await dispatcher.Send(parseResult.Request!);
        var loggedInResponse = response as Responses.LoggedInResponse;

        _phase  = GamePhase.NewAdventure;

        return loggedInResponse! with
        {
            Adventures = [new Resources.AdventureSummary("the-goblins-lair", "The Goblins' Lair")]
        };
    }

    private IResponse HandleGameStarted()
    {
        _phase = GamePhase.Login;
        return new Responses.GameStartedResponse();
    }

    private async Task<IResponse> StartAdventure(Playthrough playthrough)
    {
        var startingRoom   = await roomRepository.GetById(playthrough.AdventureId, playthrough.Location);
        var exits          = startingRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        var items          = startingRoom.Items.Select(i => i.Name).ToList();
        var lockedBarriers = GetLockedBarrierDescriptions(startingRoom.Exits, playthrough);

        return new Responses.AdventureStartedResponse(
            playthrough.ToCharacterSummary(),
            startingRoom.Name,
            startingRoom.Description,
            exits,
            items,
            lockedBarriers);
    }

    private static List<string> GetLockedBarrierDescriptions(
        IReadOnlyDictionary<Direction, Exit> roomExits,
        Playthrough                          playthrough)
    {
        var descriptions = new List<string>();
        foreach (var (_, exit) in roomExits)
        {
            if (exit.Barrier is not null && !playthrough.IsBarrierUnlocked(exit.Barrier.Id))
            {
                descriptions.Add(exit.Barrier.Description);
            }
        }

        return descriptions;
    }
}
