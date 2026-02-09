using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;

namespace Questline.Tests.Engine;

public class CommandDispatcherTests
{
    [Fact]
    public void Dispatch_RegisteredVerb_ExecutesHandler()
    {
        var world = new WorldBuilder()
            .WithRoom("start", "Start", "A starting room.")
            .Build();
        var state = new GameState(world, new Player { Location = "start" });
        var dispatcher = new CommandDispatcher();
        dispatcher.Register("look", new LookCommandHandler(), _ => new LookCommand());

        var result = dispatcher.Dispatch(state, new ParsedInput("look", []));

        result.ShouldBeOfType<LookResult>();
    }

    [Fact]
    public void Dispatch_UnknownVerb_ReturnsErrorResult()
    {
        var state = new GameState(
            new WorldBuilder().WithRoom("r", "R", "R.").Build(),
            new Player { Location = "r" });
        var dispatcher = new CommandDispatcher();

        var result = dispatcher.Dispatch(state, new ParsedInput("dance", []));

        var error = result.ShouldBeOfType<ErrorResult>();
        error.Success.ShouldBeFalse();
    }

    [Fact]
    public void Dispatch_VerbWithAlias_ExecutesHandler()
    {
        var world = new WorldBuilder()
            .WithRoom("start", "Start", "A starting room.")
            .Build();
        var state = new GameState(world, new Player { Location = "start" });
        var dispatcher = new CommandDispatcher();
        dispatcher.Register(["look", "l"], new LookCommandHandler(), _ => new LookCommand());

        var result = dispatcher.Dispatch(state, new ParsedInput("l", []));

        result.ShouldBeOfType<LookResult>();
    }

    [Fact]
    public void Dispatch_GoCommand_PassesDirectionFromArgs()
    {
        var world = new WorldBuilder()
            .WithRoom("a", "Room A", "Room A.", r => r.WithExit(Direction.North, "b"))
            .WithRoom("b", "Room B", "Room B.")
            .Build();
        var state = new GameState(world, new Player { Location = "a" });
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
    public void Dispatch_QuitCommand_ReturnsQuitResult()
    {
        var state = new GameState(
            new WorldBuilder().WithRoom("r", "R", "R.").Build(),
            new Player { Location = "r" });
        var dispatcher = new CommandDispatcher();
        dispatcher.Register("quit", new QuitCommandHandler(), _ => new QuitCommand());

        var result = dispatcher.Dispatch(state, new ParsedInput("quit", []));

        result.ShouldBeOfType<QuitResult>();
    }

    [Fact]
    public void Dispatch_EmptyVerb_ReturnsErrorResult()
    {
        var state = new GameState(
            new WorldBuilder().WithRoom("r", "R", "R.").Build(),
            new Player { Location = "r" });
        var dispatcher = new CommandDispatcher();

        var result = dispatcher.Dispatch(state, new ParsedInput("", []));

        result.ShouldBeOfType<ErrorResult>();
    }
}
