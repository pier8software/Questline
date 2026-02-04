using Shouldly;
using Xunit;

namespace Questline.Tests;

public class CommandParserTests
{
    private readonly CommandParser _parser = new();

    [Fact]
    public void Parse_VerbOnly_ReturnsCommandWithVerb()
    {
        var result = _parser.Parse("look");

        result.Verb.ShouldBe("look");
        result.Noun.ShouldBeNull();
    }

    [Fact]
    public void Parse_VerbAndNoun_ReturnsCommandWithBoth()
    {
        var result = _parser.Parse("go north");

        result.Verb.ShouldBe("go");
        result.Noun.ShouldBe("north");
    }

    [Fact]
    public void Parse_WithExtraWhitespace_TrimsInput()
    {
        var result = _parser.Parse("  look  ");

        result.Verb.ShouldBe("look");
    }

    [Fact]
    public void Parse_EmptyInput_ReturnsEmptyVerb()
    {
        var result = _parser.Parse("");

        result.Verb.ShouldBe("");
    }

    [Fact]
    public void Parse_DirectionAlias_ExpandsToGoCommand()
    {
        var result = _parser.Parse("n");

        result.Verb.ShouldBe("go");
        result.Noun.ShouldBe("north");
    }

    [Fact]
    public void Parse_InventoryAlias_ExpandsToInventory()
    {
        var result = _parser.Parse("i");

        result.Verb.ShouldBe("inventory");
    }

    [Fact]
    public void Parse_LookAlias_ExpandsToLook()
    {
        var result = _parser.Parse("l");

        result.Verb.ShouldBe("look");
    }

    [Fact]
    public void Parse_MultiWordNoun_JoinsWords()
    {
        var result = _parser.Parse("take brass lantern");

        result.Verb.ShouldBe("take");
        result.Noun.ShouldBe("brass lantern");
    }

    [Fact]
    public void Parse_UppercaseInput_ConvertsToLowercase()
    {
        var result = _parser.Parse("LOOK");

        result.Verb.ShouldBe("look");
    }

    [Theory]
    [InlineData("north", "go", "north")]
    [InlineData("south", "go", "south")]
    [InlineData("east", "go", "east")]
    [InlineData("west", "go", "west")]
    [InlineData("n", "go", "north")]
    [InlineData("s", "go", "south")]
    [InlineData("e", "go", "east")]
    [InlineData("w", "go", "west")]
    public void Parse_Direction_ConvertsToGoCommand(string input, string expectedVerb, string expectedNoun)
    {
        var result = _parser.Parse(input);

        result.Verb.ShouldBe(expectedVerb);
        result.Noun.ShouldBe(expectedNoun);
    }
}
