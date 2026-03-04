using Questline.Engine.Parsers;

namespace Questline.Tests.Engine.Parsers;

public class When_parsing_invalid_input
{
    private readonly Parser _parser = new();

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
