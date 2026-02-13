using Microsoft.Extensions.DependencyInjection;
using Questline.Cli;
using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;
using Questline.Engine.InputParsers;

namespace Questline.Tests.Cli;

public class GameLoopTests
{
    private static (GameLoop loop, FakeConsole console) CreateGameLoop()
    {
        var world = new WorldBuilder()
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

        var state = new GameState(world, new Player { Id = "player1", Location = "entrance" });

        var serviceProvider = new ServiceCollection()
            .AddSingleton<ICommandHandler<LookCommand>, LookCommandHandler>()
            .AddSingleton<ICommandHandler<GoCommand>, GoCommandHandler>()
            .AddSingleton<ICommandHandler<QuitCommand>, QuitCommandHandler>()
            .BuildServiceProvider();

        var dispatcher = new CommandDispatcher(serviceProvider);


        var console = new FakeConsole();
        var parser = new ParserBuilder()
            .RegisterCommand<LookCommand>(["look", "l"], _ => new LookCommand())
            .RegisterCommand<GoCommand>(["go", "walk", "move"], args =>
            {
                if (args.Length == 0 || !DirectionParser.TryParse(args[0], out var dir))
                {
                    return new ParseError("Invalid direction.");
                }

                return new GoCommand(dir);
            })
            .RegisterCommand<QuitCommand>(["quit", "exit", "q"], _ => new QuitCommand())
            .Build();

        var loop = new GameLoop(console, parser, dispatcher, state);

        return (loop, console);
    }

    [Fact]
    public void Displays_initial_room_on_start()
    {
        var (loop, console) = CreateGameLoop();
        console.QueueInput("quit");

        loop.Run();

        console.AllOutput.ShouldContain("Dungeon Entrance");
        console.AllOutput.ShouldContain("A dark entrance to the dungeon.");
    }

    [Fact]
    public void Displays_command_prompt()
    {
        var (loop, console) = CreateGameLoop();
        console.QueueInput("quit");

        loop.Run();

        console.AllOutput.ShouldContain("> ");
    }

    [Fact]
    public void Look_command_displays_room_info()
    {
        var (loop, console) = CreateGameLoop();
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
        var (loop, console) = CreateGameLoop();
        console.QueueInput("go north", "quit");

        loop.Run();

        console.AllOutput.ShouldContain("Torch-Lit Hallway");
    }

    [Fact]
    public void Unknown_command_displays_error()
    {
        var (loop, console) = CreateGameLoop();
        console.QueueInput("dance", "quit");

        loop.Run();

        console.AllOutput.ShouldContain("don't understand");
    }

    [Fact]
    public void Quit_command_exits_gracefully()
    {
        var (loop, console) = CreateGameLoop();
        console.QueueInput("quit");

        loop.Run();

        console.AllOutput.ShouldContain("Goodbye!");
    }

    [Fact]
    public void Null_input_exits_loop()
    {
        var (loop, console) = CreateGameLoop();
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
