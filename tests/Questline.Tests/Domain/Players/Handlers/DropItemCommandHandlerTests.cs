using Questline.Domain.Players.Entity;
using Questline.Domain.Players.Handlers;
using Questline.Domain.Players.Messages;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Tests.TestHelpers.Builders;
using Responses = Questline.Domain.Players.Messages.Responses;

namespace Questline.Tests.Domain.Players.Handlers;

public class DropItemCommandHandlerTests
{
    [Fact]
    public void Returns_successful_drop_response()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var rooms = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();

        var state = new GameState(rooms, new Player { Id = "player1", Location = "cellar" });
        state.Player.Inventory.Add(lamp);

        var handler = new DropItemCommandHandler();

        var result = handler.Handle(state, new Requests.DropItemCommand("brass lamp"));

        result.ShouldBeOfType<Responses.ItemDroppedResponse>();
        result.Message.ShouldContain("You drop the brass lamp");
    }

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

        _ = handler.Handle(state, new Requests.DropItemCommand("brass lamp"));

        state.Player.Inventory.IsEmpty.ShouldBeTrue();
        state.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBe(lamp);
    }

    [Fact]
    public void Item_not_in_inventory_returns_error_message()
    {
        var rooms = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();

        var state = new GameState(rooms, new Player { Id = "player1", Location = "cellar" });

        var handler = new DropItemCommandHandler();

        var result = handler.Handle(state, new Requests.DropItemCommand("lamp"));

        result.Message.ShouldContain("You are not carrying 'lamp'.");
    }

    [Fact]
    public void Matching_is_case_insensitive()
    {
        var rooms = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();

        var state = new GameState(rooms, new Player { Id = "player1", Location = "cellar" });

        var handler = new DropItemCommandHandler();

        var result = handler.Handle(state, new Requests.DropItemCommand("BRASS LAMP"));

        result.Message.ShouldContain("brass lamp");
    }
}
