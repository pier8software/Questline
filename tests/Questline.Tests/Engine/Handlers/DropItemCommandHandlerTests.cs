using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class DropItemCommandHandlerTests
{
    private static void GiveItemToPlayer(GameState state, Item item) => state.Player.Character.AddInventoryItem(item);

    [Fact]
    public void Returns_successful_drop_response()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .BuildState("player1", "cellar");
        GiveItemToPlayer(state, lamp);

        var handler = new DropItemCommandHandler();

        var result = handler.Handle(state, new Requests.DropItemCommand("brass lamp"));

        var dropped = result.ShouldBeOfType<Responses.ItemDroppedResponse>();
        dropped.ItemName.ShouldBe("brass lamp");
    }

    [Fact]
    public void Item_moves_from_inventory_to_room()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .BuildState("player1", "cellar");
        GiveItemToPlayer(state, lamp);
        var handler = new DropItemCommandHandler();

        _ = handler.Handle(state, new Requests.DropItemCommand("brass lamp"));

        state.Player.Character.Inventory.ShouldBeEmpty();
        state.GetRoom("cellar").FindItemByName("brass lamp").ShouldBe(lamp);
    }

    [Fact]
    public void Item_not_in_inventory_returns_error_message()
    {
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .BuildState("player1", "cellar");

        var handler = new DropItemCommandHandler();

        var result = handler.Handle(state, new Requests.DropItemCommand("lamp"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldContain("You are not carrying 'lamp'.");
    }

    [Fact]
    public void Matching_is_case_insensitive()
    {
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .BuildState("player1", "cellar");

        var handler = new DropItemCommandHandler();

        var result = handler.Handle(state, new Requests.DropItemCommand("BRASS LAMP"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldContain("BRASS LAMP");
    }
}
