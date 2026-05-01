using Microsoft.Extensions.DependencyInjection;
using Questline.Cli.Game;
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

namespace Questline.Tests.Cli.Game.GameApp;

public class When_playing_game
{
    // 4 PCs × (1 race + 18 ability scores + 1 hp + 1 occupation + 1 name) = 88 dice slots
    private static readonly int[] DefaultDiceRolls = BuildDiceRolls();

    // Login, start menu (New Game), select adventure, then party creation: accept rolled party
    private static readonly string[] SetupInputs = ["login Player1", "1", "1", "accept"];

    private static int[] BuildDiceRolls()
    {
        var rolls = new List<int>();
        for (var i = 0; i < 4; i++)
        {
            rolls.Add(1);                              // race: Human
            for (var s = 0; s < 18; s++) rolls.Add(3); // 6 × 3d6, each die = 3
            rolls.Add(2);                              // hp = 2
            rolls.Add(1);                              // occupation index
            rolls.Add(i + 1);                          // name index (unique per PC)
        }
        return rolls.ToArray();
    }

    private static (Questline.Cli.Game.GameApp app, FakeConsole console) CreateCliApp()
    {
        var rooms = new Dictionary<string, Room>
        {
            ["entrance"] = new RoomBuilder()
                .WithId("entrance")
                .WithName("Dungeon Entrance")
                .WithDescription("A dark entrance to the dungeon.")
                .WithExit(Direction.North, Exits.Default.WithDestination("hallway"))
                .Build(),
            ["hallway"] = new RoomBuilder()
                .WithId("hallway")
                .WithName("Torch-Lit Hallway")
                .WithDescription("A hallway lined with flickering torches.")
                .WithExit(Direction.South, Exits.Default.WithDestination("entrance"))
                .WithExit(Direction.North, Exits.Default.WithDestination("chamber"))
                .Build(),
            ["chamber"] = new RoomBuilder()
                .WithId("chamber")
                .WithName("Great Chamber")
                .WithDescription("A vast chamber with vaulted ceilings.")
                .WithExit(Direction.South, Exits.Default.WithDestination("hallway"))
                .Build()
        };

        var adventure = new AdventureBuilder()
            .WithId("the-goblins-lair")
            .WithName("The Goblins' Lair")
            .WithDescription("A test adventure")
            .WithStartingRoomId("entrance")
            .Build();

        var adventureRepository   = new FakeAdventureRepository(adventure);
        var roomRepository        = new FakeRoomRepository(rooms);
        var playthroughRepository = new FakePlaythroughRepository();
        var gameSession           = new GameSession();

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IGameSession>(gameSession)
            .AddSingleton<IAdventureRepository>(adventureRepository)
            .AddSingleton<IPlaythroughRepository>(playthroughRepository)
            .AddSingleton<IRoomRepository>(roomRepository)
            .AddSingleton<IRequestHandler<Requests.LoginCommand>, LoginCommandHandler>()
            .AddSingleton<IRequestHandler<Requests.GetRoomDetailsQuery>, GetRoomDetailsHandler>()
            .AddSingleton<IRequestHandler<Requests.MovePlayerCommand>, MovePlayerCommandHandler>()
            .AddSingleton<IRequestHandler<Requests.QuitGame>, QuitGameHandler>()
            .BuildServiceProvider();

        var console = new FakeConsole();

        var dice         = new FakeDice(DefaultDiceRolls);
        var stateMachine = new PartyCreationStateMachine(dice);

        var dispatcher = new RequestSender(serviceProvider);
        var parser     = new Parser();
        var gameEngine = new GameEngine(parser, dispatcher, adventureRepository, roomRepository, playthroughRepository, gameSession, stateMachine);
        var formatter  = new ResponseFormatter();

        var app = new Questline.Cli.Game.GameApp(console, formatter, gameEngine);

        return (app, console);
    }

    [Fact]
    public async Task Displays_initial_room_on_start()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput([..SetupInputs, "quit"]);

        await loop.RunAsync();

        console.AllOutput.ShouldContain("Dungeon Entrance");
        console.AllOutput.ShouldContain("A dark entrance to the dungeon.");
    }

    [Fact]
    public async Task Displays_command_prompt()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput([..SetupInputs, "quit"]);

        await loop.RunAsync();

        console.AllOutput.ShouldContain("> ");
    }

    [Fact]
    public async Task Look_command_displays_room_info()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput([..SetupInputs, "look", "quit"]);

        await loop.RunAsync();

        var output = console.AllOutput;
        var count  = CountOccurrences(output, "Dungeon Entrance");
        count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Go_command_moves_and_displays_new_room()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput([..SetupInputs, "go north", "quit"]);

        await loop.RunAsync();

        console.AllOutput.ShouldContain("Torch-Lit Hallway");
    }

    [Fact]
    public async Task Unknown_command_displays_error()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput([..SetupInputs, "dance", "quit"]);

        await loop.RunAsync();

        console.AllOutput.ShouldContain("don't understand");
    }

    [Fact]
    public async Task Quit_command_exits_gracefully()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput([..SetupInputs, "quit"]);

        await loop.RunAsync();

        console.AllOutput.ShouldContain("Goodbye!");
    }

    [Fact]
    public async Task Null_input_exits_loop()
    {
        var (loop, _) = CreateCliApp();
        // No input queued, ReadLine returns null at login prompt

        await loop.RunAsync();

        // Should not hang — exits when input is null
    }

    private static int CountOccurrences(string text, string search)
    {
        var count = 0;
        var index = 0;
        while ((index = text.IndexOf(search, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += search.Length;
        }

        return count;
    }
}
