using Questline.Domain.Characters.Entity;

namespace Questline.Tests.Domain.Characters.Entity;

public class CharacterTests
{
    private static readonly HitPoints DefaultHitPoints = new(8, 8);

    private static readonly AbilityScores DefaultAbilityScores = new(
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10),
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10));

    [Fact]
    public void Create_sets_all_character_properties()
    {
        var character = Character.Create("test-id", "TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores);

        character.Id.ShouldBe("test-id");
        character.Name.ShouldBe("TestHero");
        character.Race.ShouldBe(Race.Human);
        character.Class.ShouldBe(CharacterClass.Fighter);
        character.Level.ShouldBe(1);
        character.Experience.ShouldBe(0);
        character.HitPoints.ShouldBe(DefaultHitPoints);
        character.AbilityScores.ShouldBe(DefaultAbilityScores);
    }

    [Fact]
    public void ToSummary_returns_character_summary()
    {
        var character = Character.Create("test-id", "TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores);

        var summary = character.ToSummary();

        summary.Name.ShouldBe("TestHero");
        summary.Race.ShouldBe("Human");
        summary.Class.ShouldBe("Fighter");
        summary.Level.ShouldBe(1);
    }
}
