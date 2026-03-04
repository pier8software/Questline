using Questline.Domain.Playthroughs.Entity;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Domain.Playthroughs.Entity;

public class When_managing_inventory
{
    private readonly Playthrough _playthrough;

    public When_managing_inventory()
    {
        _playthrough = new PlaythroughBuilder().Build();
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
}
