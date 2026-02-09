using Questline.Domain;
using Questline.Engine;

namespace Questline.Tests.Engine;

public class DirectionParserTests
{
    [Theory]
    [InlineData("north", Direction.North)]
    [InlineData("south", Direction.South)]
    [InlineData("east", Direction.East)]
    [InlineData("west", Direction.West)]
    [InlineData("up", Direction.Up)]
    [InlineData("down", Direction.Down)]
    public void Parse_FullDirectionName_ReturnsDirection(string input, Direction expected)
    {
        DirectionParser.TryParse(input, out var result).ShouldBeTrue();
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("n", Direction.North)]
    [InlineData("s", Direction.South)]
    [InlineData("e", Direction.East)]
    [InlineData("w", Direction.West)]
    [InlineData("u", Direction.Up)]
    [InlineData("d", Direction.Down)]
    public void Parse_Alias_ReturnsDirection(string input, Direction expected)
    {
        DirectionParser.TryParse(input, out var result).ShouldBeTrue();
        result.ShouldBe(expected);
    }

    [Fact]
    public void Parse_InvalidDirection_ReturnsFalse() => DirectionParser.TryParse("sideways", out _).ShouldBeFalse();
}
