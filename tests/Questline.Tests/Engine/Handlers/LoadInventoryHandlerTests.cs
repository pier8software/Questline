using Questline.Domain;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;

namespace Questline.Tests.Engine.Handlers;

public class LoadInventoryHandlerTests
{
    [Fact]
    public void Lists_carried_items()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var key = new Item { Id = "key", Name = "rusty key", Description = "A rusty iron key." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        state.Player.Inventory.Add(lamp);
        state.Player.Inventory.Add(key);
        var handler = new LoadInventoryHandler();

        var result = handler.Execute(state, new Commands.LoadInventory());

        var inventoryResult = result.ShouldBeOfType<Results.InventoryLoaded>();
        inventoryResult.Items.ShouldContain("brass lamp");
        inventoryResult.Items.ShouldContain("rusty key");
    }

    [Fact]
    public void Empty_inventory_shows_not_carrying_anything()
    {
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new LoadInventoryHandler();

        var result = handler.Execute(state, new Commands.LoadInventory());

        result.Message.ShouldContain("not carrying anything");
    }

    [Fact]
    public void Get_then_drop_round_trips_item_through_inventory()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var getHandler = new TakeItemHandler();
        var dropHandler = new DropItemHandler();

        getHandler.Execute(state, new Commands.TakeItem("brass lamp"));
        state.Player.Inventory.Items.ShouldContain(lamp);
        world.GetRoom("cellar").Items.IsEmpty.ShouldBeTrue();

        dropHandler.Execute(state, new Commands.DropItem("brass lamp"));
        state.Player.Inventory.IsEmpty.ShouldBeTrue();
        world.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBe(lamp);
    }
}
