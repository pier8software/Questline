using Questline.Domain.Rooms.Entity;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.GetRoomDetailsHandler;

public class When_exit_has_barrier
{
    [Fact]
    public async Task Locked_barrier_description_is_included()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber
                .WithExit(Direction.North, Exits.WithBarrier.WithDestination("beyond")))
            .WithRoom(Rooms.BeyondRoom)
            .Build("chamber");

        var handler = new Questline.Engine.Handlers.GetRoomDetailsHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new PartyActor(), new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.LockedBarriers.ShouldContain("A heavy iron door blocks the way North.");
    }

    [Fact]
    public async Task Unlocked_barrier_is_omitted()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber
                .WithExit(Direction.North, Exits.WithBarrier.WithDestination("beyond")))
            .WithRoom(Rooms.BeyondRoom)
            .WithUnlockedBarrier("iron-door")
            .Build("chamber");

        var handler = new Questline.Engine.Handlers.GetRoomDetailsHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new PartyActor(), new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.LockedBarriers.ShouldBeEmpty();
    }
}
