using Questline.Domain.Players.Entity;
using Questline.Domain.Players.Handlers;
using Questline.Domain.Players.Messages;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Domain.Shared.Messages;
using Questline.Tests.TestHelpers.Builders;
using Responses = Questline.Domain.Players.Messages.Responses;

namespace Questline.Tests.Domain.Players.Handlers;

public class DropItemCommandHandlerTests
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
        var handler = new DropItemCommandHandler();

        var result = handler.Handle(state, new Questline.Domain.Players.Messages.Requests.DropItemCommand("brass lamp"));

        result.ShouldBeOfType<Responses.ItemDroppedResponse>();
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
        var handler = new DropItemCommandHandler();

        var result = handler.Handle(state, new Questline.Domain.Players.Messages.Requests.DropItemCommand("lamp"));

        result.ShouldBeOfType<Questline.Domain.Shared.Messages.Responses.CommandError>();
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
        var handler = new DropItemCommandHandler();

        var result = handler.Handle(state, new Questline.Domain.Players.Messages.Requests.DropItemCommand("brass lamp"));

        result.Message.ShouldContain("brass lamp");
    }
}
