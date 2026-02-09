using Questline.Domain;
using Questline.Engine;
using Questline.Engine.Commands;
using Questline.Engine.Handlers;

namespace Questline.Tests.Engine.Handlers;

public class LookCommandHandlerTests
{
    [Fact]
    public void Look_ReturnsRoomNameAndDescription()
    {
        var world = new WorldBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern with a roaring fire.")
            .Build();
        var state = new GameState(world, new Player { Location = "tavern" });
        var handler = new LookCommandHandler();

        var result = handler.Execute(state, new LookCommand());

        var lookResult = result.ShouldBeOfType<LookResult>();
        lookResult.RoomName.ShouldBe("The Tavern");
        lookResult.Description.ShouldBe("A cozy tavern with a roaring fire.");
    }

    [Fact]
    public void Look_ReturnsAvailableExits()
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
        var state = new GameState(world, new Player { Location = "hallway" });
        var handler = new LookCommandHandler();

        var result = handler.Execute(state, new LookCommand());

        var lookResult = result.ShouldBeOfType<LookResult>();
        lookResult.Exits.ShouldContain("North");
        lookResult.Exits.ShouldContain("South");
    }

    [Fact]
    public void Look_WhenNoExits_ReturnsEmptyExitsList()
    {
        var world = new WorldBuilder()
            .WithRoom("sealed-room", "Sealed Room", "No way out.")
            .Build();
        var state = new GameState(world, new Player { Location = "sealed-room" });
        var handler = new LookCommandHandler();

        var result = handler.Execute(state, new LookCommand());

        var lookResult = result.ShouldBeOfType<LookResult>();
        lookResult.Exits.ShouldBeEmpty();
    }
}
