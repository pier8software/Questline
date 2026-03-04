using Questline.Domain.Playthroughs.Entity;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Domain.Playthroughs.Entity;

public class PlaythroughTests
{
    private readonly Playthrough _playthrough;

    public PlaythroughTests()
    {
        _playthrough = new PlaythroughBuilder().Build();
    }

    [Fact]
    public void MoveTo_updates_location()
    {
        _playthrough.MoveTo("end");

        _playthrough.Location.ShouldBe("end");
    }

    [Fact]
    public void Item_is_added_to_inventory()
    {
        var lamp = Items.BrassLamp.Build();

        _playthrough.AddInventoryItem(lamp);

        _playthrough.Inventory.ShouldContain(lamp);
    }

    [Fact]
    public void Item_is_removed_from_inventory()
    {
        var lamp = Items.BrassLamp.Build();
        _playthrough.AddInventoryItem(lamp);

        _playthrough.RemoveInventoryItem(lamp);

        _playthrough.Inventory.ShouldBeEmpty();
    }

    [Fact]
    public void FindInventoryItemByName_is_case_insensitive()
    {
        var lamp = Items.BrassLamp.Build();
        _playthrough.AddInventoryItem(lamp);

        _playthrough.FindInventoryItemByName("BRASS LAMP").ShouldBe(lamp);
    }

    [Fact]
    public void FindInventoryItemByName_returns_null_when_not_found()
    {
        _playthrough.FindInventoryItemByName("sword").ShouldBeNull();
    }

    [Fact]
    public void Barrier_starts_locked()
    {
        _playthrough.IsBarrierUnlocked("iron-door").ShouldBeFalse();
    }

    [Fact]
    public void Barrier_is_unlocked_after_unlock_call()
    {
        _playthrough.UnlockBarrier("iron-door");

        _playthrough.IsBarrierUnlocked("iron-door").ShouldBeTrue();
    }

    [Fact]
    public void GetRecordedRoomItems_returns_null_for_unmodified_room()
    {
        _playthrough.GetRecordedRoomItems("cellar").ShouldBeNull();
    }

    [Fact]
    public void RecordRoomItems_stores_items_for_room()
    {
        var lamp = Items.BrassLamp.Build();

        _playthrough.RecordRoomItems("cellar", [lamp]);

        var items = _playthrough.GetRecordedRoomItems("cellar");
        items.ShouldNotBeNull();
        items!.ShouldContain(lamp);
    }

    [Fact]
    public void ToCharacterSummary_returns_summary()
    {
        var summary = _playthrough.ToCharacterSummary();

        summary.Name.ShouldBe("TestHero");
        summary.Race.ShouldBe("Human");
        summary.Class.ShouldBe("Fighter");
        summary.Level.ShouldBe(1);
    }
}
