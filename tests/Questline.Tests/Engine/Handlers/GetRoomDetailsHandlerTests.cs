using Questline.Domain.Rooms.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class GetRoomDetailsHandlerTests
{
    [Fact]
    public async Task Returns_response_with_room_details()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Hallway
                .WithItem(Items.BrassLamp)
                .WithExit(Direction.North, "throne-room")
                .WithExit(Direction.South, "entrance"))
            .WithRoom(Rooms.ThroneRoom)
            .WithRoom(Rooms.Entrance)
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
            .WithRoom(Rooms.Cellar)
            .Build("cellar");

        var handler = new GetRoomDetailsHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.Items.ShouldBeEmpty();
    }

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

            var handler = new GetRoomDetailsHandler(
                fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

            var result = await handler.Handle(new Requests.GetRoomDetailsQuery());

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

            var handler = new GetRoomDetailsHandler(
                fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

            var result = await handler.Handle(new Requests.GetRoomDetailsQuery());

            var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
            lookResult.LockedBarriers.ShouldBeEmpty();
        }
    }
}
