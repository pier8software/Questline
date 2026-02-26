using Questline.Domain.Characters.Entity;
using Questline.Domain.Shared.Entity;

namespace Questline.Tests.Domain.Characters.Entity;

public class CharacterTests
{
    private static readonly HitPoints DefaultHitPoints = new(8, 8);

    private static readonly AbilityScores DefaultAbilityScores = new(
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10),
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10));

    [Fact]
    public void Moving_the_character_updates_their_location()
    {
        var character = Character.Create("test-id", "TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");

        character.MoveTo("end");

        character.Location.ShouldBe("end");
    }

    [Fact]
    public void Can_add_a_new_item_to_a_characters_inventory()
    {
        var character = Character.Create("test-id", "TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };

        character.AddInventoryItem(lamp);

        character.Inventory.Contains(lamp).ShouldBeTrue();
    }

    [Fact]
    public void Can_remove_an_item_from_a_characters_inventory()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var character = Character.Create("test-id", "TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");
        character.AddInventoryItem(lamp);

        character.RemoveInventoryItem(lamp);

        character.Inventory.ShouldBeEmpty();
    }

    [Fact]
    public void FindInventoryItemByName_returns_item_case_insensitively()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var character = Character.Create("test-id", "TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");
        character.AddInventoryItem(lamp);

        character.FindInventoryItemByName("BRASS LAMP").ShouldBe(lamp);
    }

    [Fact]
    public void FindInventoryItemByName_returns_null_when_not_found()
    {
        var character = Character.Create("test-id", "TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");

        character.FindInventoryItemByName("sword").ShouldBeNull();
    }
}
