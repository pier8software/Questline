using Questline.Domain.Rooms.Messages;
using Questline.Engine.InputParsers;

namespace Questline.Tests.Engine.InputParser;

public class ParserTests
{
    private readonly Parser _parser = new();

    [Fact]
    public void Single_successfully_parse_verb()
    {
        var result = _parser.Parse("look");

        result.IsSuccess.ShouldBeTrue();
        result.Request.ShouldBeOfType<Requests.GetRoomDetailsQuery>();
    }

    [Fact]
    public void Alias_parsed_to_the_same_request()
    {
        var result = _parser.Parse("l");

        result.IsSuccess.ShouldBeTrue();
        result.Request.ShouldBeOfType<Requests.GetRoomDetailsQuery>();
    }

    [Fact]
    public void Verb_with_argument_splits_correctly()
    {
        var result = _parser.Parse("take lamp");

        result.IsSuccess.ShouldBeTrue();
        var command = result.Request.ShouldBeOfType<Requests.TakeItemCommand>();
        command.ItemName.ShouldBe("lamp");
    }

    [Fact]
    public void Input_is_converted_to_lowercase()
    {
        var result = _parser.Parse("TAKE LaMp");

        var command = result.Request.ShouldBeOfType<Requests.TakeItemCommand>();
        command.ItemName.ShouldBe("lamp");
    }

    [Fact]
    public void Leading_and_trailing_whitespace_is_trimmed()
    {
        var result = _parser.Parse("  look  ");

        result.Request.ShouldBeOfType<Requests.GetRoomDetailsQuery>();
    }

    [Fact]
    public void Extra_whitespace_between_words_is_ignored()
    {
        var result = _parser.Parse("take   lamp");

        var command = result.Request.ShouldBeOfType<Requests.TakeItemCommand>();
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
