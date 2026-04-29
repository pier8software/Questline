using Questline.Engine.Characters;
using Questline.Tests.TestHelpers;
using Shouldly;

namespace Questline.Tests.Engine.Characters.DiceExtensions;

public class When_check_succeeds
{
    [Fact]
    public void Returns_success_when_roll_plus_modifier_meets_dc()
    {
        var dice = new FakeDice(11);    // d20

        var result = dice.Check(modifier: 1, dc: 12);

        result.Roll.ShouldBe(11);
        result.Modifier.ShouldBe(1);
        result.DC.ShouldBe(12);
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public void Returns_success_when_roll_plus_modifier_exceeds_dc()
    {
        var dice = new FakeDice(20);

        var result = dice.Check(modifier: 0, dc: 10);

        result.Success.ShouldBeTrue();
    }
}
