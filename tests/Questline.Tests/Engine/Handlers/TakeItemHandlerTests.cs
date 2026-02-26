using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class TakeItemHandlerTests
{
    [Fact]
    public void Returns_successful_take_response()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .BuildState("player1", "cellar");

        var handler = new TakeItemHandler();

        var result = handler.Handle(state, new Requests.TakeItemCommand("brass lamp"));

        var takeResult = result.ShouldBeOfType<Responses.ItemTakenResponse>();
        takeResult.ItemName.ShouldBe("brass lamp");
    }

    [Fact]
    public void Item_moves_from_room_to_inventory()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .BuildState("player1", "cellar");
        var handler = new TakeItemHandler();

        _ = handler.Handle(state, new Requests.TakeItemCommand("brass lamp"));

        state.Character.Inventory.ShouldContain(lamp);
        state.Adventure.GetRoom("cellar").FindItemByName("brass lamp").ShouldBeNull();
    }

    [Fact]
    public void Item_not_in_room_returns_error_message()
    {
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .BuildState("player1", "cellar");

        var handler = new TakeItemHandler();

        var result = handler.Handle(state, new Requests.TakeItemCommand("lamp"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldContain("There is no 'lamp' here.");
    }

    [Fact]
    public void Matching_is_case_insensitive()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var state = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .BuildState("player1", "cellar");
        var handler = new TakeItemHandler();

        var result = handler.Handle(state, new Requests.TakeItemCommand("BRASS LAMP"));

        var takeResult = result.ShouldBeOfType<Responses.ItemTakenResponse>();
        takeResult.ItemName.ShouldBe("brass lamp");
    }
}
