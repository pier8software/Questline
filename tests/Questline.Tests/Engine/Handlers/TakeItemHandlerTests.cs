using Questline.Domain.Players.Entity;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class TakeItemHandlerTests
{
    [Fact]
    public void Returns_successful_take_response()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var rooms = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();

        var state = new GameState(rooms, new Player { Id = "player1", Location = "cellar" });

        var handler = new TakeItemHandler();

        var result = handler.Handle(state, new Requests.TakeItemCommand("brass lamp"));

        result.ShouldBeOfType<Responses.ItemTakenResponse>();
        result.Message.ShouldContain("brass lamp");
    }

    [Fact]
    public void Item_moves_from_room_to_inventory()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var rooms = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(rooms, new Player { Id = "player1", Location = "cellar" });
        var handler = new TakeItemHandler();

        _ = handler.Handle(state, new Requests.TakeItemCommand("brass lamp"));

        state.Player.Inventory.Items.ShouldContain(lamp);
        state.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBeNull();
    }

    [Fact]
    public void Item_not_in_room_returns_error_message()
    {
        var world = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();

        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });

        var handler = new TakeItemHandler();

        var result = handler.Handle(state, new Requests.TakeItemCommand("lamp"));

        result.Message.ShouldContain("There is no 'lamp' here.");
    }

    [Fact]
    public void Matching_is_case_insensitive()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new TakeItemHandler();

        var result = handler.Handle(state, new Requests.TakeItemCommand("BRASS LAMP"));

        result.Message.ShouldContain("brass lamp");
    }
}
