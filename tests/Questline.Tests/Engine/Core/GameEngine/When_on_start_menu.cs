using Microsoft.Extensions.DependencyInjection;
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
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Core.GameEngine;

public class When_on_start_menu
{
    private static int[] DefaultDiceRolls => BuildDiceRolls();

    private static int[] BuildDiceRolls()
    {
        var rolls = new List<int>();
        for (var i = 0; i < 4; i++)
        {
            rolls.Add(1);
            for (var s = 0; s < 18; s++) rolls.Add(3);
            rolls.Add(2);
            rolls.Add(1);
            rolls.Add(i + 1);
        }
        return rolls.ToArray();
    }

    private static Questline.Engine.Core.GameEngine CreateEngine(FakePlaythroughRepository? playthroughRepo = null)
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
        var stateMachine = new PartyCreationStateMachine(dice);
        var dispatcher   = new RequestSender(serviceProvider);
        var parser       = new Parser();

        return new Questline.Engine.Core.GameEngine(parser, dispatcher, adventureRepository, roomRepository, playthroughRepo, session, stateMachine);
    }

    private static async Task<Questline.Engine.Core.GameEngine> LoginAndReachStartMenu(Questline.Engine.Core.GameEngine engine)
    {
        await engine.ProcessInput(null);
        await engine.ProcessInput("login alice");
        return engine;
    }

    private static PlaythroughBuilder ThorinPlaythrough() =>
        new PlaythroughBuilder()
            .WithId("pt-1")
            .WithUsername("alice")
            .WithAdventureId("the-goblins-lair")
            .WithStartingRoomId("entrance")
            .WithCharacterName("Thorin")
            .WithLocation("entrance");

    [Fact]
    public async Task Selecting_1_transitions_to_NewAdventure()
    {
        var engine = CreateEngine();
        await LoginAndReachStartMenu(engine);

        var response = await engine.ProcessInput("1");

        engine.Phase.ShouldBe(GamePhase.NewAdventure);
        response.ShouldBeOfType<Responses.NewGameResponse>();
    }

    [Fact]
    public async Task Selecting_2_with_saves_transitions_to_LoadGame()
    {
        var playthrough = ThorinPlaythrough().Build();
        var repo = new FakePlaythroughRepository(playthrough);
        var engine = CreateEngine(repo);
        await LoginAndReachStartMenu(engine);

        var response = await engine.ProcessInput("2");

        engine.Phase.ShouldBe(GamePhase.LoadGame);
        var savedResponse = response.ShouldBeOfType<Responses.SavedPlaythroughsResponse>();
        savedResponse.Playthroughs.Count.ShouldBe(1);
        savedResponse.Playthroughs[0].CharacterName.ShouldBe("Thorin");
    }

    [Fact]
    public async Task Selecting_2_with_no_saves_stays_on_StartMenu()
    {
        var engine = CreateEngine();
        await LoginAndReachStartMenu(engine);

        var response = await engine.ProcessInput("2");

        engine.Phase.ShouldBe(GamePhase.StartMenu);
        response.ShouldBeOfType<Responses.NoSavedGamesResponse>();
    }

    [Fact]
    public async Task Invalid_input_returns_error_and_stays()
    {
        var engine = CreateEngine();
        await LoginAndReachStartMenu(engine);

        var response = await engine.ProcessInput("3");

        engine.Phase.ShouldBe(GamePhase.StartMenu);
        response.ShouldBeOfType<ErrorResponse>();
    }
}
