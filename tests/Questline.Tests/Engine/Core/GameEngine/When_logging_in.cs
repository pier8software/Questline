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

public class When_logging_in
{
    private static readonly int[] DefaultDiceRolls = [3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4];

    private static (Questline.Engine.Core.GameEngine engine, FakeGameSession session) CreateEngine()
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
        var playthroughRepo     = new FakePlaythroughRepository();
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
        var engine       = new Questline.Engine.Core.GameEngine(parser, dispatcher, adventureRepository, roomRepository, playthroughRepo, session, stateMachine);

        return (engine, session);
    }

    [Fact]
    public async Task Login_transitions_to_StartMenu()
    {
        var (engine, _) = CreateEngine();

        await engine.ProcessInput(null);
        var response = await engine.ProcessInput("login alice");

        engine.Phase.ShouldBe(GamePhase.StartMenu);
        response.ShouldBeOfType<Responses.StartMenuResponse>();
    }

    [Fact]
    public async Task Login_stores_username_on_session()
    {
        var (engine, session) = CreateEngine();

        await engine.ProcessInput(null);
        await engine.ProcessInput("login alice");

        session.Username.ShouldBe("alice");
    }
}
