using Questline.Engine;

namespace Questline.Tests.Engine;

public class ParserTests
{
    private readonly Parser _parser = new();

    [Fact]
    public void Single_word_returns_verb_with_no_args()
    {
        var result = _parser.Parse("look");

        result.Verb.ShouldBe("look");
        result.Args.ShouldBeEmpty();
    }

    [Fact]
    public void Verb_with_argument_splits_correctly()
    {
        var result = _parser.Parse("go north");

        result.Verb.ShouldBe("go");
        result.Args.ShouldBe(new[] { "north" });
    }

    [Fact]
    public void Input_is_converted_to_lowercase()
    {
        var result = _parser.Parse("GO North");

        result.Verb.ShouldBe("go");
        result.Args.ShouldBe(new[] { "north" });
    }

    [Fact]
    public void Leading_and_trailing_whitespace_is_trimmed()
    {
        var result = _parser.Parse("  look  ");

        result.Verb.ShouldBe("look");
        result.Args.ShouldBeEmpty();
    }

    [Fact]
    public void Extra_whitespace_between_words_is_ignored()
    {
        var result = _parser.Parse("go   north");

        result.Verb.ShouldBe("go");
        result.Args.ShouldBe(new[] { "north" });
    }

    [Fact]
    public void Empty_input_returns_empty_verb_and_no_args()
    {
        var result = _parser.Parse("");

        result.Verb.ShouldBe("");
        result.Args.ShouldBeEmpty();
    }
}
