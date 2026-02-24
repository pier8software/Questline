using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;

namespace Questline.Tests.Domain.Players.Entity;

public class PlayerTests
{
    private static readonly HitPoints DefaultHitPoints = new(8, 8);

    private static readonly AbilityScores DefaultAbilityScores = new(
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10),
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10));

    [Fact]
    public void Has_character_property()
    {
        var character = Character.Create("TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");
        var player = new Player("player1", character);

        player.Character.ShouldBe(character);
    }

    [Fact]
    public void Location_accessed_through_character()
    {
        var character = Character.Create("TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");
        var player = new Player("player1", character);

        player.Character.Location.ShouldBe("start");

        var moved = player with { Character = player.Character.MoveTo("end") };
        moved.Character.Location.ShouldBe("end");
    }

    [Fact]
    public void Inventory_accessed_through_character()
    {
        var character = Character.Create("TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");
        var player = new Player("player1", character);

        player.Character.Inventory.IsEmpty.ShouldBeTrue();
    }
}
