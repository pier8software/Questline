using Questline.Domain.Characters.Entity;

namespace Questline.Tests.Domain.Characters.Entity;

public class CharacterTests
{
    private static readonly HitPoints DefaultHitPoints = new(8, 8);

    private static readonly AbilityScores DefaultAbilityScores = new(
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10),
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10));

    [Fact]
    public void Location_is_mutable_via_SetLocation()
    {
        var character = Character.Create("TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");
        character.Location.ShouldBe("start");

        character.SetLocation("end");
        character.Location.ShouldBe("end");
    }

    [Fact]
    public void Inventory_starts_empty()
    {
        var character = Character.Create("TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");

        character.Inventory.IsEmpty.ShouldBeTrue();
    }
}
