using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;

namespace Questline.Tests.Engine.Handlers;

public class LookCommandHandlerTests
{
    [Fact]
    public void Returns_room_name_and_description()
    {
        var world = new WorldBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern with a roaring fire.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "tavern" });
        var handler = new LookCommandHandler();

        var result = handler.Execute(state, new LookCommand());

        var lookResult = result.ShouldBeOfType<LookResult>();
        lookResult.RoomName.ShouldBe("The Tavern");
        lookResult.Description.ShouldBe("A cozy tavern with a roaring fire.");
    }

    [Fact]
    public void Returns_available_exits()
    {
        var world = new WorldBuilder()
            .WithRoom("hallway", "Hallway", "A long hallway.", r =>
            {
                r.WithExit(Direction.North, "throne-room");
                r.WithExit(Direction.South, "entrance");
            })
            .WithRoom("throne-room", "Throne Room", "Grand throne room.")
            .WithRoom("entrance", "Entrance", "The entrance.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "hallway" });
        var handler = new LookCommandHandler();

        var result = handler.Execute(state, new LookCommand());

        var lookResult = result.ShouldBeOfType<LookResult>();
        lookResult.Exits.ShouldContain("North");
        lookResult.Exits.ShouldContain("South");
    }

    [Fact]
    public void Room_with_no_exits_returns_empty_list()
    {
        var world = new WorldBuilder()
            .WithRoom("sealed-room", "Sealed Room", "No way out.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "sealed-room" });
        var handler = new LookCommandHandler();

        var result = handler.Execute(state, new LookCommand());

        var lookResult = result.ShouldBeOfType<LookResult>();
        lookResult.Exits.ShouldBeEmpty();
    }

    [Fact]
    public void Room_with_items_includes_them_in_result()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new LookCommandHandler();

        var result = handler.Execute(state, new LookCommand());

        var lookResult = result.ShouldBeOfType<LookResult>();
        lookResult.Items.ShouldContain("brass lamp");
        lookResult.Message.ShouldContain("You can see");
    }

    [Fact]
    public void Room_with_no_items_omits_items_line()
    {
        var world = new WorldBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build();
        var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
        var handler = new LookCommandHandler();

        var result = handler.Execute(state, new LookCommand());

        result.Message.ShouldNotContain("You can see");
    }
}
