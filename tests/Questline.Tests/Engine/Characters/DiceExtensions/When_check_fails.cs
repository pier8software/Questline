using Questline.Engine.Characters;
using Questline.Tests.TestHelpers;
using Shouldly;

namespace Questline.Tests.Engine.Characters.DiceExtensions;

public class When_check_fails
{
    [Fact]
    public void Returns_failure_when_roll_plus_modifier_below_dc()
    {
        var dice = new FakeDice(5);

        var result = dice.Check(modifier: 2, dc: 12);

        result.Roll.ShouldBe(5);
        result.Modifier.ShouldBe(2);
        result.DC.ShouldBe(12);
        result.Success.ShouldBeFalse();
    }
}
