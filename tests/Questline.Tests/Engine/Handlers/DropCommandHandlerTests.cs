using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;

namespace Questline.Tests.Engine.Handlers;

public class DropCommandHandlerTests
{
    [Fact]
    public void Drop_WhenItemInInventory_PlacesInRoomAndRemovesFromInventory()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        state.Player.Inventory.Add(lamp);
        var handler = new DropCommandHandler();

        var result = handler.Execute(state, new DropCommand("brass lamp"));

        result.ShouldBeOfType<ItemDroppedResult>();
        state.Player.Inventory.IsEmpty.ShouldBeTrue();
        world.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBe(lamp);
    }

    [Fact]
    public void Drop_WhenItemNotInInventory_ReturnsError()
    {
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new DropCommandHandler();

        var result = handler.Execute(state, new DropCommand("lamp"));

        result.ShouldBeOfType<ErrorResult>();
        result.Success.ShouldBeFalse();
    }

    [Fact]
    public void Drop_ReturnsDescriptiveMessage()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        state.Player.Inventory.Add(lamp);
        var handler = new DropCommandHandler();

        var result = handler.Execute(state, new DropCommand("brass lamp"));

        result.Message.ShouldContain("brass lamp");
    }
}
