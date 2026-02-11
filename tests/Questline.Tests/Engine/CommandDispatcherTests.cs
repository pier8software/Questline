using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;

namespace Questline.Tests.Engine;

public class CommandDispatcherTests
{
    [Fact]
    public void Registered_verb_executes_its_handler()
    {
        var world = new WorldBuilder()
            .WithRoom("start", "Start", "A starting room.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "start" });
        var dispatcher = new CommandDispatcher();
        dispatcher.Register("look", new LookCommandHandler(), _ => new LookCommand());

        var result = dispatcher.Dispatch(state, new ParsedInput("look", []));

        result.ShouldBeOfType<LookResult>();
    }

    [Fact]
    public void Unknown_verb_returns_error()
    {
        var state = new GameState(
            new WorldBuilder().WithRoom("r", "R", "R.").Build(),
            new Player { Id = "player1", Location = "r" });
        var dispatcher = new CommandDispatcher();

        var result = dispatcher.Dispatch(state, new ParsedInput("dance", []));

        var error = result.ShouldBeOfType<ErrorResult>();
        error.Success.ShouldBeFalse();
    }

    [Fact]
    public void Alias_executes_the_same_handler()
    {
        var world = new WorldBuilder()
            .WithRoom("start", "Start", "A starting room.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "start" });
        var dispatcher = new CommandDispatcher();
        dispatcher.Register(["look", "l"], new LookCommandHandler(), _ => new LookCommand());

        var result = dispatcher.Dispatch(state, new ParsedInput("l", []));

        result.ShouldBeOfType<LookResult>();
    }

    [Fact]
    public void Go_command_passes_direction_from_args()
    {
        var world = new WorldBuilder()
            .WithRoom("a", "Room A", "Room A.", r => r.WithExit(Direction.North, "b"))
            .WithRoom("b", "Room B", "Room B.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "a" });
        var dispatcher = new CommandDispatcher();
        dispatcher.Register(["go", "walk", "move"], new GoCommandHandler(), args =>
        {
            if (args.Length == 0 || !DirectionParser.TryParse(args[0], out var dir))
            {
                return null;
            }

            return new GoCommand(dir);
        });

        var result = dispatcher.Dispatch(state, new ParsedInput("go", ["north"]));

        result.ShouldBeOfType<MovedResult>();
        state.Player.Location.ShouldBe("b");
    }

    [Fact]
    public void Quit_command_returns_quit_result()
    {
        var state = new GameState(
            new WorldBuilder().WithRoom("r", "R", "R.").Build(),
            new Player { Id = "player1", Location = "r" });
        var dispatcher = new CommandDispatcher();
        dispatcher.Register("quit", new QuitCommandHandler(), _ => new QuitCommand());

        var result = dispatcher.Dispatch(state, new ParsedInput("quit", []));

        result.ShouldBeOfType<QuitResult>();
    }

    [Fact]
    public void Empty_verb_returns_error()
    {
        var state = new GameState(
            new WorldBuilder().WithRoom("r", "R", "R.").Build(),
            new Player { Id = "player1", Location = "r" });
        var dispatcher = new CommandDispatcher();

        var result = dispatcher.Dispatch(state, new ParsedInput("", []));

        result.ShouldBeOfType<ErrorResult>();
    }
}
