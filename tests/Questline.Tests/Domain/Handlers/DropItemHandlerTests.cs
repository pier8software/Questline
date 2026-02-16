using Questline.Domain.Entities;
using Questline.Domain.Handlers;
using Questline.Domain.Messages;
using Questline.Domain.Shared;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Domain.Handlers;

public class DropItemHandlerTests
{
    [Fact]
    public void Item_moves_from_inventory_to_room()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var rooms = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(rooms, new Player { Id = "player1", Location = "cellar" });
        state.Player.Inventory.Add(lamp);
        var handler = new DropItemHandler();

        var result = handler.Execute(state, new Commands.DropItem("brass lamp"));

        result.ShouldBeOfType<Results.ItemDropped>();
        state.Player.Inventory.IsEmpty.ShouldBeTrue();
        state.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBe(lamp);
    }

    [Fact]
    public void Item_not_in_inventory_returns_error()
    {
        var rooms = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(rooms, new Player { Id = "player1", Location = "cellar" });
        var handler = new DropItemHandler();

        var result = handler.Execute(state, new Commands.DropItem("lamp"));

        result.ShouldBeOfType<Results.CommandError>();
        result.Success.ShouldBeFalse();
    }

    [Fact]
    public void Result_message_mentions_item_name()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var rooms = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(rooms, new Player { Id = "player1", Location = "cellar" });
        state.Player.Inventory.Add(lamp);
        var handler = new DropItemHandler();

        var result = handler.Execute(state, new Commands.DropItem("brass lamp"));

        result.Message.ShouldContain("brass lamp");
    }
}
