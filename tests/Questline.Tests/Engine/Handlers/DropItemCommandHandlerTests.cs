using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class DropItemCommandHandlerTests
{
    [Fact]
    public void Returns_successful_drop_response()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var rooms = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();

        var state = new GameState(rooms, new Player { Id = "player1", Character = new Character("TestHero", Race.Human, CharacterClass.Fighter) { Location = "cellar" } });
        state.Player.Character.Inventory.Add(lamp);

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
        var state = new GameState(rooms, new Player { Id = "player1", Character = new Character("TestHero", Race.Human, CharacterClass.Fighter) { Location = "cellar" } });
        state.Player.Character.Inventory.Add(lamp);
        var handler = new DropItemCommandHandler();

        _ = handler.Handle(state, new Requests.DropItemCommand("brass lamp"));

        state.Player.Character.Inventory.IsEmpty.ShouldBeTrue();
        state.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBe(lamp);
    }

    [Fact]
    public void Item_not_in_inventory_returns_error_message()
    {
        var rooms = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();

        var state = new GameState(rooms, new Player { Id = "player1", Character = new Character("TestHero", Race.Human, CharacterClass.Fighter) { Location = "cellar" } });

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

        var state = new GameState(rooms, new Player { Id = "player1", Character = new Character("TestHero", Race.Human, CharacterClass.Fighter) { Location = "cellar" } });

        var handler = new DropItemCommandHandler();

        var result = handler.Handle(state, new Requests.DropItemCommand("BRASS LAMP"));

        result.Message.ShouldContain("brass lamp");
    }
}
