using Questline.Domain.Messages;
using Questline.Domain.Players.Entity;
using Questline.Domain.Players.Handlers;
using Questline.Domain.Players.Messages;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Domain.Players.Handlers;

public class DropPlayerItemHandlerTests
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
        var handler = new DropPlayerItemHandler();

        var result = handler.Execute(state, new Questline.Domain.Players.Messages.Commands.DropPlayerItem("brass lamp"));

        result.ShouldBeOfType<Events.PlayerItemDropped>();
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
        var handler = new DropPlayerItemHandler();

        var result = handler.Execute(state, new Questline.Domain.Players.Messages.Commands.DropPlayerItem("lamp"));

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
        var handler = new DropPlayerItemHandler();

        var result = handler.Execute(state, new Questline.Domain.Players.Messages.Commands.DropPlayerItem("brass lamp"));

        result.Message.ShouldContain("brass lamp");
    }
}
