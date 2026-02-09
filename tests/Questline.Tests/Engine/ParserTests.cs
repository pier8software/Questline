using Questline.Engine;

namespace Questline.Tests.Engine;

public class ParserTests
{
    private readonly Parser _parser = new();

    [Fact]
    public void Parse_SingleWord_ReturnsVerbWithNoArgs()
    {
        var result = _parser.Parse("look");

        result.Verb.ShouldBe("look");
        result.Args.ShouldBeEmpty();
    }

    [Fact]
    public void Parse_VerbWithArgument_SplitsCorrectly()
    {
        var result = _parser.Parse("go north");

        result.Verb.ShouldBe("go");
        result.Args.ShouldBe(new[] { "north" });
    }

    [Fact]
    public void Parse_ConvertsToLowercase()
    {
        var result = _parser.Parse("GO North");

        result.Verb.ShouldBe("go");
        result.Args.ShouldBe(new[] { "north" });
    }

    [Fact]
    public void Parse_TrimsWhitespace()
    {
        var result = _parser.Parse("  look  ");

        result.Verb.ShouldBe("look");
        result.Args.ShouldBeEmpty();
    }

    [Fact]
    public void Parse_MultipleSpaces_IgnoresExtraWhitespace()
    {
        var result = _parser.Parse("go   north");

        result.Verb.ShouldBe("go");
        result.Args.ShouldBe(new[] { "north" });
    }

    [Fact]
    public void Parse_EmptyInput_ReturnsEmptyVerbAndNoArgs()
    {
        var result = _parser.Parse("");

        result.Verb.ShouldBe("");
        result.Args.ShouldBeEmpty();
    }
}
