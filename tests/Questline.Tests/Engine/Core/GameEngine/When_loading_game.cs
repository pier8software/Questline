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
using static Questline.Tests.TestHelpers.Builders.Templates;
using Microsoft.Extensions.DependencyInjection;

namespace Questline.Tests.Engine.Core;

public class When_loading_game
{
    private static readonly int[] DefaultDiceRolls = [3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4];

    private static (GameEngine engine, FakeGameSession session) CreateEngine(FakePlaythroughRepository playthroughRepo)
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

        return (engine, session);
    }

    private static PlaythroughBuilder ThorinPlaythrough() =>
        new PlaythroughBuilder()
            .WithId("pt-1")
            .WithUsername("alice")
            .WithAdventureId("the-goblins-lair")
            .WithStartingRoomId("entrance")
            .WithCharacterName("Thorin")
            .WithLocation("entrance");

    private static async Task<GameEngine> LoginAndReachLoadGame(GameEngine engine)
    {
        await engine.ProcessInput(null);
        await engine.ProcessInput("login alice");
        await engine.ProcessInput("2");
        return engine;
    }

    [Fact]
    public async Task Selecting_valid_playthrough_transitions_to_Playing()
    {
        var playthrough = ThorinPlaythrough().Build();
        var repo = new FakePlaythroughRepository(playthrough);
        var (engine, session) = CreateEngine(repo);
        await LoginAndReachLoadGame(engine);

        var response = await engine.ProcessInput("1");

        engine.Phase.ShouldBe(GamePhase.Playing);
        session.PlaythroughId.ShouldBe("pt-1");
        var adventureStarted = response.ShouldBeOfType<Responses.AdventureStartedResponse>();
        adventureStarted.RoomName.ShouldBe("Dungeon Entrance");
    }

    [Fact]
    public async Task Invalid_selection_returns_error_and_stays()
    {
        var playthrough = ThorinPlaythrough().Build();
        var repo = new FakePlaythroughRepository(playthrough);
        var (engine, _) = CreateEngine(repo);
        await LoginAndReachLoadGame(engine);

        var response = await engine.ProcessInput("99");

        engine.Phase.ShouldBe(GamePhase.LoadGame);
        response.ShouldBeOfType<ErrorResponse>();
    }
}
