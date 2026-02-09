using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;

namespace Questline.Tests.Engine.Handlers;

public class InventoryCommandHandlerTests
{
    [Fact]
    public void Inventory_WhenCarryingItems_ListsThem()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var key = new Item { Id = "key", Name = "rusty key", Description = "A rusty iron key." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        state.Player.Inventory.Add(lamp);
        state.Player.Inventory.Add(key);
        var handler = new InventoryCommandHandler();

        var result = handler.Execute(state, new InventoryCommand());

        var inventoryResult = result.ShouldBeOfType<InventoryResult>();
        inventoryResult.Items.ShouldContain("brass lamp");
        inventoryResult.Items.ShouldContain("rusty key");
    }

    [Fact]
    public void Inventory_WhenEmpty_ShowsMessage()
    {
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new InventoryCommandHandler();

        var result = handler.Execute(state, new InventoryCommand());

        result.Message.ShouldContain("not carrying anything");
    }

    [Fact]
    public void GetThenDrop_MovesItemFromRoomToInventoryAndBack()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var getHandler = new GetCommandHandler();
        var dropHandler = new DropCommandHandler();

        getHandler.Execute(state, new GetCommand("brass lamp"));
        state.Player.Inventory.Items.ShouldContain(lamp);
        world.GetRoom("cellar").Items.IsEmpty.ShouldBeTrue();

        dropHandler.Execute(state, new DropCommand("brass lamp"));
        state.Player.Inventory.IsEmpty.ShouldBeTrue();
        world.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBe(lamp);
    }
}
