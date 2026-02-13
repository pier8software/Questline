using Questline.Domain;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;

namespace Questline.Tests.Engine.Handlers;

public class GetCommandHandlerTests
{
    [Fact]
    public void Item_moves_from_room_to_inventory()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new GetCommandHandler();

        var result = handler.Execute(state, new Commands.GetCommand("brass lamp"));

        result.ShouldBeOfType<Results.ItemPickedUpResult>();
        state.Player.Inventory.Items.ShouldContain(lamp);
        world.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBeNull();
    }

    [Fact]
    public void Item_not_in_room_returns_error()
    {
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new GetCommandHandler();

        var result = handler.Execute(state, new Commands.GetCommand("lamp"));

        result.ShouldBeOfType<Results.ErrorResult>();
        result.Success.ShouldBeFalse();
    }

    [Fact]
    public void Matching_is_case_insensitive()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new GetCommandHandler();

        var result = handler.Execute(state, new Commands.GetCommand("BRASS LAMP"));

        result.ShouldBeOfType<Results.ItemPickedUpResult>();
    }

    [Fact]
    public void Result_message_mentions_item_name()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new GetCommandHandler();

        var result = handler.Execute(state, new Commands.GetCommand("brass lamp"));

        result.Message.ShouldContain("brass lamp");
    }
}
