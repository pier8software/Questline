using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;

namespace Questline.Tests.Engine.Handlers;

public class GetCommandHandlerTests
{
    [Fact]
    public void Get_WhenItemInRoom_AddsToInventoryAndRemovesFromRoom()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new GetCommandHandler();

        var result = handler.Execute(state, new GetCommand("brass lamp"));

        result.ShouldBeOfType<ItemPickedUpResult>();
        state.Player.Inventory.Items.ShouldContain(lamp);
        world.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBeNull();
    }

    [Fact]
    public void Get_WhenItemNotInRoom_ReturnsError()
    {
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new GetCommandHandler();

        var result = handler.Execute(state, new GetCommand("lamp"));

        result.ShouldBeOfType<ErrorResult>();
        result.Success.ShouldBeFalse();
    }

    [Fact]
    public void Get_IsCaseInsensitive()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new GetCommandHandler();

        var result = handler.Execute(state, new GetCommand("BRASS LAMP"));

        result.ShouldBeOfType<ItemPickedUpResult>();
    }

    [Fact]
    public void Get_ReturnsDescriptiveMessage()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new GetCommandHandler();

        var result = handler.Execute(state, new GetCommand("brass lamp"));

        result.Message.ShouldContain("brass lamp");
    }
}
