using Questline.Domain.Characters.Entity;
using Shouldly;

namespace Questline.Tests.Domain.Characters.Entity.AbilityScore;

public class When_calculating_modifier
{
    [Theory]
    [InlineData(3, -4)]
    [InlineData(8, -1)]
    [InlineData(9, -1)]
    [InlineData(10, 0)]
    [InlineData(11, 0)]
    [InlineData(12, 1)]
    [InlineData(13, 1)]
    [InlineData(14, 2)]
    [InlineData(18, 4)]
    public void Modifier_matches_dnd_formula(int score, int expectedModifier)
    {
        var ability = new Questline.Domain.Characters.Entity.AbilityScore(score);

        ability.Modifier.ShouldBe(expectedModifier);
    }
}
