using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Engine.Handlers;

public class GetRoomDetailsHandlerTests
{
    [Fact]
    public async Task Returns_response_with_room_details()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var fixture = new GameBuilder()
            .WithRoom("hallway", "Hallway", "A long hallway.", r =>
            {
                r.WithItem(lamp);
                r.WithExit(Direction.North, "throne-room");
                r.WithExit(Direction.South, "entrance");
            })
            .WithRoom("throne-room", "Throne Room", "Grand throne room.")
            .WithRoom("entrance",    "Entrance",    "The entrance.")
            .Build("hallway");

        var handler = new GetRoomDetailsHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.RoomName.ShouldBe("Hallway");
        lookResult.Description.ShouldBe("A long hallway.");
        lookResult.Items.ShouldContain("brass lamp");
        lookResult.Exits.ShouldContain("North");
        lookResult.Exits.ShouldContain("South");
    }

    [Fact]
    public async Task Response_omits_items_when_room_is_empty()
    {
        var fixture = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build("cellar");

        var handler = new GetRoomDetailsHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.Items.ShouldBeEmpty();
    }

    [Fact]
    public async Task Response_includes_locked_barrier_description()
    {
        var barrier = new Barrier
        {
            Id             = "iron-door",
            Name           = "iron door",
            Description    = "A heavy iron door blocks the way North.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId   = "rusty-key",
            UnlockMessage  = "The rusty key turns in the lock..."
        };

        var fixture = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.",
                r => r.WithExit(Direction.North, new Exit("beyond", barrier)))
            .WithRoom("beyond", "Beyond", "Beyond the door.")
            .Build("chamber");

        var handler = new GetRoomDetailsHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.LockedBarriers.ShouldContain("A heavy iron door blocks the way North.");
    }

    [Fact]
    public async Task Response_omits_barrier_line_when_unlocked()
    {
        var barrier = new Barrier
        {
            Id             = "iron-door",
            Name           = "iron door",
            Description    = "A heavy iron door blocks the way North.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId   = "rusty-key",
            UnlockMessage  = "The rusty key turns in the lock..."
        };

        var fixture = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.",
                r => r.WithExit(Direction.North, new Exit("beyond", barrier)))
            .WithRoom("beyond", "Beyond", "Beyond the door.")
            .WithUnlockedBarrier("iron-door")
            .Build("chamber");

        var handler = new GetRoomDetailsHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.LockedBarriers.ShouldBeEmpty();
    }
}
