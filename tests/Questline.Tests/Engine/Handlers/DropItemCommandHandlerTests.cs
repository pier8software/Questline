using Questline.Domain.Shared.Entity;
using Questline.Engine.Core;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class DropItemCommandHandlerTests
{
    private static void GiveItemToPlayer(GameState state, Item item)
    {
        state.Character.AddInventoryItem(item);
    }

    [Fact]
    public async Task Returns_successful_drop_response()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .BuildState("player1", "cellar");
        GiveItemToPlayer(state, lamp);

        var handler = new DropItemCommandHandler();

        var result = await handler.Handle(state, new Requests.DropItemCommand("brass lamp"));

        var dropResult = result.ShouldBeOfType<Responses.ItemDroppedResponse>();
        dropResult.ItemName.ShouldBe("brass lamp");
    }

    [Fact]
    public async Task Item_moves_from_inventory_to_room()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .BuildState("player1", "cellar");
        GiveItemToPlayer(state, lamp);
        var handler = new DropItemCommandHandler();

        _ = await handler.Handle(state, new Requests.DropItemCommand("brass lamp"));

        state.Character.Inventory.ShouldBeEmpty();
        state.Adventure.GetRoom("cellar").FindItemByName("brass lamp").ShouldBe(lamp);
    }

    [Fact]
    public async Task Item_not_in_inventory_returns_error()
    {
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .BuildState("player1", "cellar");

        var handler = new DropItemCommandHandler();

        var result = await handler.Handle(state, new Requests.DropItemCommand("lamp"));

        var dropResult = result.ShouldBeOfType<Responses.ItemDroppedResponse>();
        dropResult.ItemName.ShouldContain("You are not carrying 'lamp'.");
    }

    [Fact]
    public async Task Matching_is_case_insensitive()
    {
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .BuildState("player1", "cellar");

        var handler = new DropItemCommandHandler();

        var result = await handler.Handle(state, new Requests.DropItemCommand("BRASS LAMP"));

        var dropResult = result.ShouldBeOfType<Responses.ItemDroppedResponse>();
        dropResult.ItemName.ShouldContain("BRASS LAMP");
    }
}
