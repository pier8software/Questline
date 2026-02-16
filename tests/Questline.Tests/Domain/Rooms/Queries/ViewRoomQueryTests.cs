using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Rooms.Messages;
using Questline.Domain.Rooms.Queries;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Domain.Rooms.Queries;

public class ViewRoomQueryTests
{
    [Fact]
    public void Returns_room_name_and_description()
    {
        var world = new GameBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern with a roaring fire.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "tavern" });
        var handler = new ViewRoomQuery();

        var result = handler.Execute(state, new Questline.Domain.Rooms.Messages.Commands.ViewRoom());

        var lookResult = result.ShouldBeOfType<Events.RoomViewed>();
        lookResult.RoomName.ShouldBe("The Tavern");
        lookResult.Description.ShouldBe("A cozy tavern with a roaring fire.");
    }

    [Fact]
    public void Returns_available_exits()
    {
        var world = new GameBuilder()
            .WithRoom("hallway", "Hallway", "A long hallway.", r =>
            {
                r.WithExit(Direction.North, "throne-room");
                r.WithExit(Direction.South, "entrance");
            })
            .WithRoom("throne-room", "Throne Room", "Grand throne room.")
            .WithRoom("entrance", "Entrance", "The entrance.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "hallway" });
        var handler = new ViewRoomQuery();

        var result = handler.Execute(state, new Questline.Domain.Rooms.Messages.Commands.ViewRoom());

        var lookResult = result.ShouldBeOfType<Events.RoomViewed>();
        lookResult.Exits.ShouldContain("North");
        lookResult.Exits.ShouldContain("South");
    }

    [Fact]
    public void Room_with_no_exits_returns_empty_list()
    {
        var world = new GameBuilder()
            .WithRoom("sealed-room", "Sealed Room", "No way out.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "sealed-room" });
        var handler = new ViewRoomQuery();

        var result = handler.Execute(state, new Questline.Domain.Rooms.Messages.Commands.ViewRoom());

        var lookResult = result.ShouldBeOfType<Events.RoomViewed>();
        lookResult.Exits.ShouldBeEmpty();
    }

    [Fact]
    public void Room_with_items_includes_them_in_result()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new ViewRoomQuery();

        var result = handler.Execute(state, new Questline.Domain.Rooms.Messages.Commands.ViewRoom());

        var lookResult = result.ShouldBeOfType<Events.RoomViewed>();
        lookResult.Items.ShouldContain("brass lamp");
        lookResult.Message.ShouldContain("You can see");
    }

    [Fact]
    public void Room_with_no_items_omits_items_line()
    {
        var world = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new ViewRoomQuery();

        var result = handler.Execute(state, new Questline.Domain.Rooms.Messages.Commands.ViewRoom());

        result.Message.ShouldNotContain("You can see");
    }
}
