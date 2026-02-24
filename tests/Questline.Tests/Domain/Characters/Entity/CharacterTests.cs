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
    public void MoveTo_returns_new_character_with_updated_location()
    {
        var character = Character.Create("TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");

        var moved = character.MoveTo("end");

        moved.Location.ShouldBe("end");
    }

    [Fact]
    public void MoveTo_leaves_original_character_unchanged()
    {
        var character = Character.Create("TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");

        _ = character.MoveTo("end");

        character.Location.ShouldBe("start");
    }

    [Fact]
    public void Inventory_starts_empty()
    {
        var character = Character.Create("TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");

        character.Inventory.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void AddInventoryItem_returns_new_character_with_item_in_inventory()
    {
        var character = Character.Create("TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };

        var updated = character.AddInventoryItem(lamp);

        updated.Inventory.Contains(lamp).ShouldBeTrue();
    }

    [Fact]
    public void AddInventoryItem_leaves_original_character_unchanged()
    {
        var character = Character.Create("TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start");
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };

        _ = character.AddInventoryItem(lamp);

        character.Inventory.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void RemoveInventoryItem_returns_new_character_without_item()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var character = Character.Create("TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start")
            .AddInventoryItem(lamp);

        var updated = character.RemoveInventoryItem(lamp);

        updated.Inventory.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void RemoveInventoryItem_leaves_original_character_unchanged()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var character = Character.Create("TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, "start")
            .AddInventoryItem(lamp);

        _ = character.RemoveInventoryItem(lamp);

        character.Inventory.Contains(lamp).ShouldBeTrue();
    }
}
