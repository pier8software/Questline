using Questline.Cli;
using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;

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

        var state = new GameState(world, new Player { Location = "entrance" });

        var dispatcher = new CommandDispatcher();
        dispatcher.Register(["look", "l"], new LookCommandHandler(), _ => new LookCommand());
        dispatcher.Register(["go", "walk", "move"], new GoCommandHandler(), args =>
        {
            if (args.Length == 0 || !DirectionParser.TryParse(args[0], out var dir))
            {
                return null;
            }

            return new GoCommand(dir);
        });
        dispatcher.Register(["quit", "exit", "q"], new QuitCommandHandler(), _ => new QuitCommand());

        var console = new FakeConsole();
        var parser = new Parser();
        var loop = new GameLoop(console, parser, dispatcher, state);

        return (loop, console);
    }

    [Fact]
    public void GameLoop_DisplaysInitialRoomOnStart()
    {
        var (loop, console) = CreateGameLoop();
        console.QueueInput("quit");

        loop.Run();

        console.AllOutput.ShouldContain("Dungeon Entrance");
        console.AllOutput.ShouldContain("A dark entrance to the dungeon.");
    }

    [Fact]
    public void GameLoop_DisplaysPrompt()
    {
        var (loop, console) = CreateGameLoop();
        console.QueueInput("quit");

        loop.Run();

        console.AllOutput.ShouldContain("> ");
    }

    [Fact]
    public void GameLoop_LookCommand_DisplaysRoomInfo()
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
    public void GameLoop_GoCommand_MovesAndDisplaysNewRoom()
    {
        var (loop, console) = CreateGameLoop();
        console.QueueInput("go north", "quit");

        loop.Run();

        console.AllOutput.ShouldContain("Torch-Lit Hallway");
    }

    [Fact]
    public void GameLoop_UnknownCommand_DisplaysError()
    {
        var (loop, console) = CreateGameLoop();
        console.QueueInput("dance", "quit");

        loop.Run();

        console.AllOutput.ShouldContain("don't understand");
    }

    [Fact]
    public void GameLoop_QuitCommand_ExitsGracefully()
    {
        var (loop, console) = CreateGameLoop();
        console.QueueInput("quit");

        loop.Run();

        console.AllOutput.ShouldContain("Goodbye!");
    }

    [Fact]
    public void GameLoop_NullInput_ExitsLoop()
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
