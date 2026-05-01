using Questline.Domain.Characters.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Tests.TestHelpers.Builders;
using Shouldly;

namespace Questline.Tests.Domain.Characters.Entity.Character;

public class When_managing_inventory
{
    [Fact]
    public void Add_item_appears_in_inventory()
    {
        var aric = CharacterBuilder.New().Build();
        var key  = new Item { Id = "rusty-key", Name = "rusty key", Description = "" };

        aric.AddInventoryItem(key);

        aric.Inventory.ShouldContain(key);
    }

    [Fact]
    public void Remove_item_takes_it_out_of_inventory()
    {
        var aric = CharacterBuilder.New().Build();
        var key  = new Item { Id = "rusty-key", Name = "rusty key", Description = "" };
        aric.AddInventoryItem(key);

        aric.RemoveInventoryItem(key);

        aric.Inventory.ShouldNotContain(key);
    }

    [Fact]
    public void Find_by_name_is_case_insensitive()
    {
        var aric = CharacterBuilder.New().Build();
        var key  = new Item { Id = "rusty-key", Name = "rusty key", Description = "" };
        aric.AddInventoryItem(key);

        aric.FindInventoryItemByName("RUSTY KEY").ShouldBe(key);
    }
}
