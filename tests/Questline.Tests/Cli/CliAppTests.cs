using Microsoft.Extensions.DependencyInjection;
using Questline.Cli;
using Questline.Domain.Handlers;
using Questline.Domain.Messages;
using Questline.Domain.Players.Entity;
using Questline.Domain.Players.Handlers;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Rooms.Queries;
using Questline.Domain.Shared.Data;
using Questline.Engine;
using Questline.Engine.InputParsers;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;

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
            .AddSingleton<ICommandHandler<Questline.Domain.Rooms.Messages.Commands.ViewRoom>, ViewRoomQuery>()
            .AddSingleton<ICommandHandler<Questline.Domain.Players.Messages.Commands.MovePlayer>, MovePlayerHandler>()
            .AddSingleton<ICommandHandler<Commands.QuitGame>, QuitGameHandler>()
            .BuildServiceProvider();

        var dispatcher = new CommandDispatcher(serviceProvider);


        var console = new FakeConsole();
        var parser = new ParserBuilder()
            .RegisterCommand<Questline.Domain.Rooms.Messages.Commands.ViewRoom>(["look", "l"], _ => new Questline.Domain.Rooms.Messages.Commands.ViewRoom())
            .RegisterCommand<Questline.Domain.Players.Messages.Commands.MovePlayer>(["go", "walk", "move"], args =>
            {
                if (args.Length == 0 || !DirectionParser.TryParse(args[0], out var dir))
                {
                    return new ParseError("Invalid direction.");
                }

                return new Questline.Domain.Players.Messages.Commands.MovePlayer(dir);
            })
            .RegisterCommand<Commands.QuitGame>(["quit", "exit", "q"], _ => new Commands.QuitGame())
            .Build();

        var app = new CliApp(console, new GameEngine(parser, dispatcher, state));

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
