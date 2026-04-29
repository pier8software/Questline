using Questline.Domain.Characters.Entity;
using DomainHitPoints     = Questline.Domain.Characters.Entity.HitPoints;
using DomainAbilityScore  = Questline.Domain.Characters.Entity.AbilityScore;
using DomainAbilityScores = Questline.Domain.Characters.Entity.AbilityScores;
using DomainCharacter     = Questline.Domain.Characters.Entity.Character;

namespace Questline.Tests.Domain.Characters.Entity.Character;

public class When_converting_to_summary
{
    private static readonly DomainHitPoints DefaultHitPoints = new(max: 8, current: 8);

    private static readonly DomainAbilityScores DefaultAbilityScores = new(
        new DomainAbilityScore(10), new DomainAbilityScore(10), new DomainAbilityScore(10),
        new DomainAbilityScore(10), new DomainAbilityScore(10), new DomainAbilityScore(10));

    [Fact]
    public void ToSummary_returns_character_summary()
    {
        var character = DomainCharacter.Create("test-id", "TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores);

        var summary = character.ToSummary();

        summary.Name.ShouldBe("TestHero");
        summary.Race.ShouldBe("Human");
        summary.Class.ShouldBe("Fighter");
        summary.Level.ShouldBe(1);
    }
}
