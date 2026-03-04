using Questline.Domain.Rooms.Entity;
using Questline.Engine.Characters;
using Questline.Engine.Core;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Engine.Parsers;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;
using Microsoft.Extensions.DependencyInjection;

namespace Questline.Tests.Engine.Core;

public class GameEngineStartMenuTests
{
    private static readonly int[] DefaultDiceRolls = [3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4];

    private static (GameEngine engine, FakePlaythroughRepository playthroughRepo, FakeGameSession session) CreateEngine(
        FakePlaythroughRepository? playthroughRepo = null)
    {
        var rooms = new Dictionary<string, Room>
        {
            ["entrance"] = Rooms.DungeonEntrance
                .WithExit(Direction.North, "hallway")
                .Build()
        };

        var adventure = new AdventureBuilder()
            .WithId("the-goblins-lair")
            .WithName("The Goblins' Lair")
            .WithDescription("A test adventure")
            .WithStartingRoomId("entrance")
            .Build();

        var adventureRepository = new FakeAdventureRepository(adventure);
        var roomRepository      = new FakeRoomRepository(rooms);
        playthroughRepo       ??= new FakePlaythroughRepository();
        var session             = new FakeGameSession("test");

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IGameSession>(session)
            .AddSingleton<IAdventureRepository>(adventureRepository)
            .AddSingleton<IPlaythroughRepository>(playthroughRepo)
            .AddSingleton<IRoomRepository>(roomRepository)
            .AddSingleton<IRequestHandler<Requests.LoginCommand>, LoginCommandHandler>()
            .BuildServiceProvider();

        var dice         = new FakeDice(DefaultDiceRolls);
        var stateMachine = new CharacterCreationStateMachine(dice);
        var dispatcher   = new RequestSender(serviceProvider);
        var parser       = new Parser();
        var engine       = new GameEngine(parser, dispatcher, adventureRepository, roomRepository, playthroughRepo, session, stateMachine);

        return (engine, playthroughRepo, session);
    }

    private static PlaythroughBuilder ThorinPlaythrough() =>
        new PlaythroughBuilder()
            .WithId("pt-1")
            .WithUsername("alice")
            .WithAdventureId("the-goblins-lair")
            .WithStartingRoomId("entrance")
            .WithCharacterName("Thorin")
            .WithLocation("entrance");

    private static async Task<GameEngine> LoginAndReachStartMenu(GameEngine engine)
    {
        await engine.ProcessInput(null);         // Started -> Login
        await engine.ProcessInput("login alice"); // Login -> StartMenu
        return engine;
    }

    [Fact]
    public async Task Login_transitions_to_StartMenu()
    {
        var (engine, _, _) = CreateEngine();

        await engine.ProcessInput(null);          // Started -> Login
        var response = await engine.ProcessInput("login alice");

        engine.Phase.ShouldBe(GamePhase.StartMenu);
        response.ShouldBeOfType<Responses.StartMenuResponse>();
    }

    [Fact]
    public async Task Login_stores_username_on_session()
    {
        var (engine, _, session) = CreateEngine();

        await engine.ProcessInput(null);
        await engine.ProcessInput("login alice");

        session.Username.ShouldBe("alice");
    }

    [Fact]
    public async Task StartMenu_selecting_1_transitions_to_NewAdventure()
    {
        var (engine, _, _) = CreateEngine();
        await LoginAndReachStartMenu(engine);

        var response = await engine.ProcessInput("1");

        engine.Phase.ShouldBe(GamePhase.NewAdventure);
        response.ShouldBeOfType<Responses.NewGameResponse>();
    }

    [Fact]
    public async Task StartMenu_selecting_2_with_saves_transitions_to_LoadGame()
    {
        var playthrough = ThorinPlaythrough().Build();
        var repo = new FakePlaythroughRepository(playthrough);
        var (engine, _, _) = CreateEngine(repo);
        await LoginAndReachStartMenu(engine);

        var response = await engine.ProcessInput("2");

        engine.Phase.ShouldBe(GamePhase.LoadGame);
        var savedResponse = response.ShouldBeOfType<Responses.SavedPlaythroughsResponse>();
        savedResponse.Playthroughs.Count.ShouldBe(1);
        savedResponse.Playthroughs[0].CharacterName.ShouldBe("Thorin");
    }

    [Fact]
    public async Task StartMenu_selecting_2_with_no_saves_stays_on_StartMenu()
    {
        var (engine, _, _) = CreateEngine();
        await LoginAndReachStartMenu(engine);

        var response = await engine.ProcessInput("2");

        engine.Phase.ShouldBe(GamePhase.StartMenu);
        response.ShouldBeOfType<Responses.NoSavedGamesResponse>();
    }

    [Fact]
    public async Task StartMenu_invalid_input_returns_error_and_stays()
    {
        var (engine, _, _) = CreateEngine();
        await LoginAndReachStartMenu(engine);

        var response = await engine.ProcessInput("3");

        engine.Phase.ShouldBe(GamePhase.StartMenu);
        response.ShouldBeOfType<ErrorResponse>();
    }

    [Fact]
    public async Task LoadGame_selecting_valid_playthrough_transitions_to_Playing()
    {
        var playthrough = ThorinPlaythrough().Build();
        var repo = new FakePlaythroughRepository(playthrough);
        var (engine, _, session) = CreateEngine(repo);
        await LoginAndReachStartMenu(engine);
        await engine.ProcessInput("2"); // -> LoadGame

        var response = await engine.ProcessInput("1");

        engine.Phase.ShouldBe(GamePhase.Playing);
        session.PlaythroughId.ShouldBe("pt-1");
        var adventureStarted = response.ShouldBeOfType<Responses.AdventureStartedResponse>();
        adventureStarted.RoomName.ShouldBe("Dungeon Entrance");
    }

    [Fact]
    public async Task LoadGame_invalid_selection_returns_error_and_stays()
    {
        var playthrough = ThorinPlaythrough().Build();
        var repo = new FakePlaythroughRepository(playthrough);
        var (engine, _, _) = CreateEngine(repo);
        await LoginAndReachStartMenu(engine);
        await engine.ProcessInput("2"); // -> LoadGame

        var response = await engine.ProcessInput("99");

        engine.Phase.ShouldBe(GamePhase.LoadGame);
        response.ShouldBeOfType<ErrorResponse>();
    }
}
