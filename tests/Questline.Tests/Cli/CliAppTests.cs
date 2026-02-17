using Microsoft.Extensions.DependencyInjection;
using Questline.Cli;
using Questline.Domain.Players.Entity;
using Questline.Domain.Players.Handlers;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Rooms.Handlers;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Handlers;
using Questline.Engine;
using Questline.Engine.InputParsers;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Domain.Rooms.Messages.Requests;
using static Questline.Domain.Players.Messages.Requests;
using static Questline.Domain.Shared.Messages.Requests;

namespace Questline.Tests.Cli;

public class CliAppTests
{
    private static (CliApp app, FakeConsole console) CreateCliApp()
    {
        var rooms = new GameBuilder()
            .WithRoom("entrance", "Dungeon Entrance", "A dark entrance to the dungeon.", r =>
                r.WithExit(Direction.North, "hallway"))
            .WithRoom("hallway", "Torch-Lit Hallway", "A hallway lined with flickering torches.", r =>
            {
                r.WithExit(Direction.South, "entrance");
                r.WithExit(Direction.North, "chamber");
            })
            .WithRoom("chamber", "Great Chamber", "A vast chamber with vaulted ceilings.", r =>
                r.WithExit(Direction.South, "hallway"))
            .Build();

        var state = new GameState(rooms, new Player { Id = "player1", Location = "entrance" });

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IRequestHandler<GetRoomDetailsQuery>, GetRoomDetailsHandler>()
            .AddSingleton<IRequestHandler<MovePlayerCommand>, MovePlayerCommandHandler>()
            .AddSingleton<IRequestHandler<QuitGame>, QuitGameHandler>()
            .BuildServiceProvider();

        var dispatcher = new RequestSender(serviceProvider);


        var console = new FakeConsole();
        var parser = new Parser();
        var gameEngine = new GameEngine(parser, dispatcher, state);

        var app = new CliApp(console, gameEngine);

        return (app, console);
    }

    [Fact]
    public void Displays_initial_room_on_start()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput("quit");

        loop.Run();

        console.AllOutput.ShouldContain("Dungeon Entrance");
        console.AllOutput.ShouldContain("A dark entrance to the dungeon.");
    }

    [Fact]
    public void Displays_command_prompt()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput("quit");

        loop.Run();

        console.AllOutput.ShouldContain("> ");
    }

    [Fact]
    public void Look_command_displays_room_info()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput("look", "quit");

        loop.Run();

        var output = console.AllOutput;
        // The initial display + look should both show room info
        var count = CountOccurrences(output, "Dungeon Entrance");
        count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void Go_command_moves_and_displays_new_room()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput("go north", "quit");

        loop.Run();

        console.AllOutput.ShouldContain("Torch-Lit Hallway");
    }

    [Fact]
    public void Unknown_command_displays_error()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput("dance", "quit");

        loop.Run();

        console.AllOutput.ShouldContain("don't understand");
    }

    [Fact]
    public void Quit_command_exits_gracefully()
    {
        var (loop, console) = CreateCliApp();
        console.QueueInput("quit");

        loop.Run();

        console.AllOutput.ShouldContain("Goodbye!");
    }

    [Fact]
    public void Null_input_exits_loop()
    {
        var (loop, console) = CreateCliApp();
        // No input queued, ReadLine returns null

        loop.Run();

        // Should not hang — loop exits when input is null
        console.AllOutput.ShouldContain("Dungeon Entrance");
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
