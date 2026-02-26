using Questline.Engine.Parsers;
using static Questline.Engine.Messages.Requests;

namespace Questline.Tests.Engine.Parsers;

public class ParserTests
{
    private readonly Parser _parser = new();

    [Theory]
    [InlineData("drop",       typeof(DropItemCommand))]
    [InlineData("examine",    typeof(ExamineCommand))]
    [InlineData("inspect",    typeof(ExamineCommand))]
    [InlineData("look",       typeof(GetRoomDetailsQuery))]
    [InlineData("l",          typeof(GetRoomDetailsQuery))]
    [InlineData("inventory",  typeof(GetPlayerInventoryQuery))]
    [InlineData("inv",        typeof(GetPlayerInventoryQuery))]
    [InlineData("i",          typeof(GetPlayerInventoryQuery))]
    [InlineData("go north",   typeof(MovePlayerCommand))]
    [InlineData("move north", typeof(MovePlayerCommand))]
    [InlineData("walk north", typeof(MovePlayerCommand))]
    [InlineData("take",       typeof(TakeItemCommand))]
    [InlineData("get",        typeof(TakeItemCommand))]
    [InlineData("quit",       typeof(QuitGame))]
    [InlineData("exit",       typeof(QuitGame))]
    [InlineData("use",        typeof(UseItemCommand))]
    [InlineData("version",    typeof(VersionQuery))]
    [InlineData("ver",        typeof(VersionQuery))]
    public void Input_is_parsed_correctly(string input, Type expectedType)
    {
        var result = _parser.Parse(input);

        result.IsSuccess.ShouldBeTrue();
        result.Request.ShouldBeOfType(expectedType);
    }

    [Fact]
    public void Input_is_converted_to_lowercase()
    {
        var result = _parser.Parse("LOOK");

        var command = result.Request.ShouldBeOfType<GetRoomDetailsQuery>();
    }

    [Fact]
    public void Leading_and_trailing_whitespace_is_trimmed()
    {
        var result = _parser.Parse("  look  ");

        result.Request.ShouldBeOfType<GetRoomDetailsQuery>();
    }

    [Fact]
    public void Extra_whitespace_between_words_is_ignored()
    {
        var result = _parser.Parse("take   lamp");

        var command = result.Request.ShouldBeOfType<TakeItemCommand>();
        command.ItemName.ShouldBe("lamp");
    }

    [Fact]
    public void Empty_input_returns_parse_error()
    {
        var result = _parser.Parse("");

        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Message.ShouldBe("Please enter a command.");
    }

    [Fact]
    public void Unknown_verb_returns_error()
    {
        var result = _parser.Parse("error");

        result.IsSuccess.ShouldBeFalse();
        result.Error!.Message.ShouldBe("I don't understand 'error'.");
    }
}
