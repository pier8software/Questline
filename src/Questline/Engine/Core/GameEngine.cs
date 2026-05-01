using Questline.Domain.Parties.Entity;
using Questline.Domain.Playthroughs.Data;
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
    StartMenu,
    NewAdventure,
    LoadGame,
    PartyCreation,
    Playing,
    Ended
}

public class GameEngine(
    Parser                     parser,
    RequestSender              dispatcher,
    IAdventureRepository       adventureRepository,
    IRoomRepository            roomRepository,
    IPlaythroughRepository     playthroughRepository,
    IGameSession               gameSession,
    PartyCreationStateMachine  stateMachine)
{
    private GamePhase                                        _phase               = GamePhase.Started;
    private string                                           _selectedAdventureId = null!;
    private IReadOnlyDictionary<int, PlaythroughSummary>?    _savedPlaythroughs;

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
            case GamePhase.StartMenu:
                return await HandleStartMenu(input);
            case GamePhase.NewAdventure:
                return await HandleNewAdventure(input);
            case GamePhase.LoadGame:
                return await HandleLoadGame(input);
            case GamePhase.PartyCreation:
                return await HandlePartyCreation(input);
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
        var playthrough = await playthroughRepository.GetById(gameSession.PlaythroughId!);
        var parseResult = parser.Parse(input, playthrough.Party);
        if (!parseResult.IsSuccess)
        {
            return parseResult.Error!;
        }

        var response = await dispatcher.Send(parseResult.Actor!, parseResult.Request!);

        playthrough.IncrementTurns();
        await playthroughRepository.Save(playthrough);

        if (response is Responses.GameQuitedResponse)
        {
            _phase = GamePhase.Ended;
        }

        return response;
    }

    private async Task<IResponse> HandlePartyCreation(string? input)
    {
        var response = stateMachine.ProcessInput(input);

        if (stateMachine.CompletedParty is { } party)
        {
            var adventure   = await adventureRepository.GetById(_selectedAdventureId);
            var playthrough = Playthrough.Create(
                gameSession.Username!,
                _selectedAdventureId,
                adventure.StartingRoomId,
                party);

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
        _phase               = GamePhase.PartyCreation;

        return Task.FromResult(stateMachine.Start() as IResponse);
    }

    private async Task<IResponse> HandleLogin(string? input)
    {
        var parseResult = parser.Parse(input);
        if (!parseResult.IsSuccess)
        {
            return parseResult.Error!;
        }

        var response         = await dispatcher.Send(new NoActor(), parseResult.Request!);
        var loggedInResponse = response as Responses.LoggedInResponse;

        gameSession.SetUsername(loggedInResponse!.Player.Username);
        _phase = GamePhase.StartMenu;

        return new Responses.StartMenuResponse();
    }

    private async Task<IResponse> HandleStartMenu(string? input)
    {
        switch (input)
        {
            case "1":
                _phase = GamePhase.NewAdventure;
                return new Responses.NewGameResponse(
                    [new Resources.AdventureSummary("the-goblins-lair", "The Goblins' Lair")]);
            case "2":
                var playthroughs = await playthroughRepository.FindByUsername(gameSession.Username!);
                if (playthroughs.Count == 0)
                {
                    return new Responses.NoSavedGamesResponse();
                }

                _savedPlaythroughs = playthroughs
                    .Select((p, i) => (Index: i + 1, Summary: p))
                    .ToDictionary(x => x.Index, x => x.Summary);
                _phase = GamePhase.LoadGame;
                return new Responses.SavedPlaythroughsResponse(playthroughs);
            default:
                return new ErrorResponse("Invalid selection.");
        }
    }

    private async Task<IResponse> HandleLoadGame(string? input)
    {
        if (!int.TryParse(input, out var selection) ||
            _savedPlaythroughs is null ||
            !_savedPlaythroughs.TryGetValue(selection, out var summary))
        {
            return new ErrorResponse("Invalid selection.");
        }

        var playthrough = await playthroughRepository.GetById(summary.Id);
        gameSession.SetPlaythroughId(playthrough.Id);
        _phase = GamePhase.Playing;
        return await StartAdventure(playthrough);
    }

    private IResponse HandleGameStarted()
    {
        _phase = GamePhase.Login;
        return new Responses.GameStartedResponse();
    }

    private async Task<IResponse> StartAdventure(Playthrough playthrough)
    {
        var startingRoom   = await roomRepository.GetById(playthrough.Location);
        var exits          = startingRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        var items          = startingRoom.Items.Select(i => i.Name).ToList();
        var lockedBarriers = GetLockedBarrierDescriptions(startingRoom.Exits, playthrough);

        return new Responses.AdventureStartedResponse(
            playthrough.Party.Members[0].ToSummary(),
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
