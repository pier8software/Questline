using Shouldly;
using Xunit;

namespace Questline.Tests;

public class CommandHandlerTests
{
    private readonly CommandHandler _handler = new();
    private readonly GameState _initialState = new();

    [Fact]
    public void Execute_Look_ReturnsRoomDescription()
    {
        var command = new Command("look");

        var result = _handler.Execute(command, _initialState);

        result.Output.ShouldContain("clearing");
        result.NewState.ShouldBe(_initialState);
    }

    [Fact]
    public void Execute_GoNorth_ChangesRoom()
    {
        var command = new Command("go", "north");

        var result = _handler.Execute(command, _initialState);

        result.NewState.CurrentRoomId.ShouldBe("forest");
        result.Output.ShouldContain("forest");
    }

    [Fact]
    public void Execute_GoInvalidDirection_ReturnsError()
    {
        var command = new Command("go", "up");

        var result = _handler.Execute(command, _initialState);

        result.Output.ShouldBe("You can't go that way.");
        result.NewState.CurrentRoomId.ShouldBe("start");
    }

    [Fact]
    public void Execute_GoWithoutDirection_AsksWhere()
    {
        var command = new Command("go");

        var result = _handler.Execute(command, _initialState);

        result.Output.ShouldBe("Go where?");
    }

    [Fact]
    public void Execute_InventoryEmpty_ReturnsEmptyHanded()
    {
        var command = new Command("inventory");

        var result = _handler.Execute(command, _initialState);

        result.Output.ShouldBe("You are empty-handed.");
    }

    [Fact]
    public void Execute_Quit_SetsQuitFlag()
    {
        var command = new Command("quit");

        var result = _handler.Execute(command, _initialState);

        result.NewState.Flags.GetValueOrDefault("quit").ShouldBeTrue();
        result.Output.ShouldContain("Thanks");
    }

    [Fact]
    public void Execute_UnknownCommand_ReturnsError()
    {
        var command = new Command("dance");

        var result = _handler.Execute(command, _initialState);

        result.Output.ShouldContain("don't understand");
    }

    [Fact]
    public void Execute_EmptyCommand_ReturnsPardon()
    {
        var command = new Command("");

        var result = _handler.Execute(command, _initialState);

        result.Output.ShouldBe("I beg your pardon?");
    }

    [Fact]
    public void Execute_Help_ReturnsHelpText()
    {
        var command = new Command("help");

        var result = _handler.Execute(command, _initialState);

        result.Output.ShouldContain("Available commands");
        result.Output.ShouldContain("look");
        result.Output.ShouldContain("go");
    }

    [Fact]
    public void Execute_GoMarksRoomAsVisited()
    {
        var command = new Command("go", "north");

        var result = _handler.Execute(command, _initialState);

        result.NewState.Rooms["forest"].Visited.ShouldBeTrue();
    }

    [Fact]
    public void DescribeRoom_ReturnsCurrentRoomDescription()
    {
        var description = _handler.DescribeRoom(_initialState);

        description.ShouldContain("clearing");
    }
}
