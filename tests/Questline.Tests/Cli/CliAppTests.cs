using Microsoft.Extensions.DependencyInjection;
using Questline.Cli;
using Questline.Domain.Adventures.Entity;
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

namespace Questline.Tests.Cli;

public class CliAppTests
{
    // 3d6 x 6 ability scores = 18 rolls, then 1d8 for HP = 19 rolls total
    private static readonly int[] DefaultDiceRolls = [3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4];

    // Login, select adventure, then character creation: select class (Fighter), select race (Human), continue (HP roll), enter name
    private static readonly string[] SetupInputs = ["login Player1", "1", "1", "1", "", "Hero"];

    private static (CliApp app, FakeConsole console) CreateCliApp()
    {
        var rooms = new Dictionary<string, Room>
        {
            ["entrance"] = new RoomBuilder("entrance", "Dungeon Entrance", "A dark entrance to the dungeon.")
                .WithExit(Direction.North, "hallway")
                .Build(),
            ["hallway"] = new RoomBuilder("hallway", "Torch-Lit Hallway", "A hallway lined with flickering torches.")
                .WithExit(Direction.South, "entrance")
                .WithExit(Direction.North, "chamber")
                .Build(),
            ["chamber"] = new RoomBuilder("chamber", "Great Chamber", "A vast chamber with vaulted ceilings.")
                .WithExit(Direction.South, "hallway")
                .Build()
        };

        var adventure = new Adventure
        {
            Id             = "the-goblins-lair",
            Name           = "The Goblins' Lair",
            Description    = "A test adventure",
            StartingRoomId = "entrance"
        };

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
        var stateMachine = new CharacterCreationStateMachine(dice);

        var dispatcher = new RequestSender(serviceProvider);
        var parser     = new Parser();
        var gameEngine = new GameEngine(parser, dispatcher, adventureRepository, roomRepository, playthroughRepository, gameSession, stateMachine);
        var formatter  = new ResponseFormatter();

        var app = new CliApp(console, formatter, gameEngine);

        return (app, console);
    }

    [Fact]
    public async Task Displays_initial_room_on_start()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput([..SetupInputs, "quit"]);

        await loop.Run();

        console.AllOutput.ShouldContain("Dungeon Entrance");
        console.AllOutput.ShouldContain("A dark entrance to the dungeon.");
    }

    [Fact]
    public async Task Displays_command_prompt()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput([..SetupInputs, "quit"]);

        await loop.Run();

        console.AllOutput.ShouldContain("> ");
    }

    [Fact]
    public async Task Look_command_displays_room_info()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput([..SetupInputs, "look", "quit"]);

        await loop.Run();

        var output = console.AllOutput;
        var count  = CountOccurrences(output, "Dungeon Entrance");
        count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Go_command_moves_and_displays_new_room()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput([..SetupInputs, "go north", "quit"]);

        await loop.Run();

        console.AllOutput.ShouldContain("Torch-Lit Hallway");
    }

    [Fact]
    public async Task Unknown_command_displays_error()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput([..SetupInputs, "dance", "quit"]);

        await loop.Run();

        console.AllOutput.ShouldContain("don't understand");
    }

    [Fact]
    public async Task Quit_command_exits_gracefully()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput([..SetupInputs, "quit"]);

        await loop.Run();

        console.AllOutput.ShouldContain("Goodbye!");
    }

    [Fact]
    public async Task Null_input_exits_loop()
    {
        var (loop, console) = CreateCliApp();
        // No input queued, ReadLine returns null at login prompt

        await loop.Run();

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
