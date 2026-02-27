using Questline.Domain.Characters.Entity;
using Questline.Domain.Playthroughs.Entity;
using Questline.Domain.Shared.Entity;

namespace Questline.Tests.Domain.Playthroughs.Entity;

public class PlaythroughTests
{
    private static readonly HitPoints DefaultHitPoints = new(8, 8);

    private static readonly AbilityScores DefaultAbilityScores = new(
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10),
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10));

    private static Playthrough CreatePlaythrough(string location = "start") => new()
    {
        Id             = "pt-1",
        AdventureId    = "test-adventure",
        StartingRoomId = "start",
        CharacterName  = "TestHero",
        Race           = Race.Human,
        Class          = CharacterClass.Fighter,
        AbilityScores  = DefaultAbilityScores,
        HitPoints      = DefaultHitPoints,
        Location       = location
    };

    [Fact]
    public void Create_from_character_captures_all_data()
    {
        var character = Character.Create("char-1", "TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores);

        var playthrough = Playthrough.Create("test-adventure", "start", character);

        playthrough.AdventureId.ShouldBe("test-adventure");
        playthrough.StartingRoomId.ShouldBe("start");
        playthrough.CharacterName.ShouldBe("TestHero");
        playthrough.Race.ShouldBe(Race.Human);
        playthrough.Class.ShouldBe(CharacterClass.Fighter);
        playthrough.Location.ShouldBe("start");
        playthrough.Inventory.ShouldBeEmpty();
        playthrough.UnlockedBarriers.ShouldBeEmpty();
    }

    [Fact]
    public void MoveTo_updates_location()
    {
        var playthrough = CreatePlaythrough();

        playthrough.MoveTo("end");

        playthrough.Location.ShouldBe("end");
    }

    [Fact]
    public void Can_add_item_to_inventory()
    {
        var playthrough = CreatePlaythrough();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };

        playthrough.AddInventoryItem(lamp);

        playthrough.Inventory.ShouldContain(lamp);
    }

    [Fact]
    public void Can_remove_item_from_inventory()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var playthrough = CreatePlaythrough();
        playthrough.AddInventoryItem(lamp);

        playthrough.RemoveInventoryItem(lamp);

        playthrough.Inventory.ShouldBeEmpty();
    }

    [Fact]
    public void FindInventoryItemByName_is_case_insensitive()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var playthrough = CreatePlaythrough();
        playthrough.AddInventoryItem(lamp);

        playthrough.FindInventoryItemByName("BRASS LAMP").ShouldBe(lamp);
    }

    [Fact]
    public void FindInventoryItemByName_returns_null_when_not_found()
    {
        var playthrough = CreatePlaythrough();

        playthrough.FindInventoryItemByName("sword").ShouldBeNull();
    }

    [Fact]
    public void Barrier_starts_locked()
    {
        var playthrough = CreatePlaythrough();

        playthrough.IsBarrierUnlocked("iron-door").ShouldBeFalse();
    }

    [Fact]
    public void Can_unlock_barrier()
    {
        var playthrough = CreatePlaythrough();

        playthrough.UnlockBarrier("iron-door");

        playthrough.IsBarrierUnlocked("iron-door").ShouldBeTrue();
    }

    [Fact]
    public void GetRecordedRoomItems_returns_null_for_unmodified_room()
    {
        var playthrough = CreatePlaythrough();

        playthrough.GetRecordedRoomItems("cellar").ShouldBeNull();
    }

    [Fact]
    public void RecordRoomItems_stores_items_for_room()
    {
        var playthrough = CreatePlaythrough();
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };

        playthrough.RecordRoomItems("cellar", [lamp]);

        var items = playthrough.GetRecordedRoomItems("cellar");
        items.ShouldNotBeNull();
        items!.ShouldContain(lamp);
    }

    [Fact]
    public void ToCharacterSummary_returns_summary()
    {
        var playthrough = CreatePlaythrough();

        var summary = playthrough.ToCharacterSummary();

        summary.Name.ShouldBe("TestHero");
        summary.Race.ShouldBe("Human");
        summary.Class.ShouldBe("Fighter");
        summary.Level.ShouldBe(1);
    }
}
