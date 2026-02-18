using Questline.Engine.InputParsers;
using static Questline.Engine.Messages.Requests;

namespace Questline.Tests.Engine.InputParser;

public class ParserTests
{
    private readonly Parser _parser = new();

    [Fact]
    public void Single_successfully_parse_verb()
    {
        var result = _parser.Parse("look");

        result.IsSuccess.ShouldBeTrue();
        result.Request.ShouldBeOfType<GetRoomDetailsQuery>();
    }

    [Fact]
    public void Alias_parsed_to_the_same_request()
    {
        var result = _parser.Parse("l");

        result.IsSuccess.ShouldBeTrue();
        result.Request.ShouldBeOfType<GetRoomDetailsQuery>();
    }

    [Fact]
    public void Verb_with_argument_splits_correctly()
    {
        var result = _parser.Parse("take lamp");

        result.IsSuccess.ShouldBeTrue();
        var command = result.Request.ShouldBeOfType<TakeItemCommand>();
        command.ItemName.ShouldBe("lamp");
    }

    [Fact]
    public void Input_is_converted_to_lowercase()
    {
        var result = _parser.Parse("TAKE LaMp");

        var command = result.Request.ShouldBeOfType<TakeItemCommand>();
        command.ItemName.ShouldBe("lamp");
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

    [Fact]
    public void Use_with_target_parses_correctly()
    {
        var result = _parser.Parse("use rusty key on iron door");

        result.IsSuccess.ShouldBeTrue();
        var command = result.Request.ShouldBeOfType<UseItemCommand>();
        command.ItemName.ShouldBe("rusty key");
        command.TargetName.ShouldBe("iron door");
    }

    [Fact]
    public void Use_without_target_parses_correctly()
    {
        var result = _parser.Parse("use rusty key");

        result.IsSuccess.ShouldBeTrue();
        var command = result.Request.ShouldBeOfType<UseItemCommand>();
        command.ItemName.ShouldBe("rusty key");
        command.TargetName.ShouldBeNull();
    }

    [Fact]
    public void Examine_parses_correctly()
    {
        var result = _parser.Parse("examine rusty key");

        result.IsSuccess.ShouldBeTrue();
        var command = result.Request.ShouldBeOfType<ExamineCommand>();
        command.TargetName.ShouldBe("rusty key");
    }

    [Fact]
    public void Examine_alias_x_parses_correctly()
    {
        var result = _parser.Parse("x torch");

        result.IsSuccess.ShouldBeTrue();
        var command = result.Request.ShouldBeOfType<ExamineCommand>();
        command.TargetName.ShouldBe("torch");
    }

    [Fact]
    public void Inspect_alias_parses_correctly()
    {
        var result = _parser.Parse("inspect symbols");

        result.IsSuccess.ShouldBeTrue();
        var command = result.Request.ShouldBeOfType<ExamineCommand>();
        command.TargetName.ShouldBe("symbols");
    }
}
