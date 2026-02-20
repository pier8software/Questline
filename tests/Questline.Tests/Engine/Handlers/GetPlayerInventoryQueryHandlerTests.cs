using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class GetPlayerInventoryQueryHandlerTests
{
    [Fact]
    public void Lists_carried_items()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var key = new Item { Id = "key", Name = "rusty key", Description = "A rusty iron key." };
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .BuildState("player1", "cellar");
        state.Player.Character.Inventory.Add(lamp);
        state.Player.Character.Inventory.Add(key);
        var handler = new GetPlayerInventoryQueryHandler();

        var result = handler.Handle(state, new Requests.GetPlayerInventoryQuery());

        var inventoryResult = result.ShouldBeOfType<Responses.PlayerInventoryResponse>();
        inventoryResult.Items.ShouldContain("brass lamp");
        inventoryResult.Items.ShouldContain("rusty key");
    }

    [Fact]
    public void Empty_inventory_shows_not_carrying_anything()
    {
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .BuildState("player1", "cellar");
        var handler = new GetPlayerInventoryQueryHandler();

        var result = handler.Handle(state, new Requests.GetPlayerInventoryQuery());

        result.Message.ShouldContain("not carrying anything");
    }

    [Fact]
    public void Get_then_drop_round_trips_item_through_inventory()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .BuildState("player1", "cellar");
        var getHandler = new TakeItemHandler();
        var dropHandler = new DropItemCommandHandler();

        getHandler.Handle(state, new Requests.TakeItemCommand("brass lamp"));
        state.Player.Character.Inventory.Items.ShouldContain(lamp);
        state.GetRoom("cellar").Items.IsEmpty.ShouldBeTrue();

        dropHandler.Handle(state, new Requests.DropItemCommand("brass lamp"));
        state.Player.Character.Inventory.IsEmpty.ShouldBeTrue();
        state.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBe(lamp);
    }
}
