using Questline.Domain.Messages;
using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Handlers;
using Questline.Domain.Rooms.Messages;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Domain.Rooms.Handlers;

public class TakeRoomItemHandlerTests
{
    [Fact]
    public void Item_moves_from_room_to_inventory()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var rooms = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(rooms, new Player { Id = "player1", Location = "cellar" });
        var handler = new TakeRoomItemHandler();

        var result = handler.Execute(state, new Questline.Domain.Rooms.Messages.Commands.TakeRoomItem("brass lamp"));

        result.ShouldBeOfType<Events.RoomItemTaken>();
        state.Player.Inventory.Items.ShouldContain(lamp);
        state.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBeNull();
    }

    [Fact]
    public void Item_not_in_room_returns_error()
    {
        var world = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new TakeRoomItemHandler();

        var result = handler.Execute(state, new Questline.Domain.Rooms.Messages.Commands.TakeRoomItem("lamp"));

        result.ShouldBeOfType<Results.CommandError>();
        result.Success.ShouldBeFalse();
    }

    [Fact]
    public void Matching_is_case_insensitive()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new TakeRoomItemHandler();

        var result = handler.Execute(state, new Questline.Domain.Rooms.Messages.Commands.TakeRoomItem("BRASS LAMP"));

        result.ShouldBeOfType<Events.RoomItemTaken>();
    }

    [Fact]
    public void Result_message_mentions_item_name()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new TakeRoomItemHandler();

        var result = handler.Execute(state, new Questline.Domain.Rooms.Messages.Commands.TakeRoomItem("brass lamp"));

        result.Message.ShouldContain("brass lamp");
    }
}
