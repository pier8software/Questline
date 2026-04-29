using Questline.Domain.Characters.Entity;

namespace Questline.Tests.Domain.Characters.Entity.Character;

public class When_converting_to_summary
{
    private static readonly Questline.Domain.Characters.Entity.HitPoints DefaultHitPoints = new(max: 8, current: 8);

    private static readonly AbilityScores DefaultAbilityScores = new(
        new Questline.Domain.Characters.Entity.AbilityScore(10), new Questline.Domain.Characters.Entity.AbilityScore(10), new Questline.Domain.Characters.Entity.AbilityScore(10),
        new Questline.Domain.Characters.Entity.AbilityScore(10), new Questline.Domain.Characters.Entity.AbilityScore(10), new Questline.Domain.Characters.Entity.AbilityScore(10));

    [Fact]
    public void ToSummary_returns_character_summary()
    {
        var character = Questline.Domain.Characters.Entity.Character.Create("test-id", "TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores);

        var summary = character.ToSummary();

        summary.Name.ShouldBe("TestHero");
        summary.Race.ShouldBe("Human");
        summary.Class.ShouldBe("Fighter");
        summary.Level.ShouldBe(1);
    }
}
