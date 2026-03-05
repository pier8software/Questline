using Questline.Domain.Rooms.Entity;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.GetRoomDetailsHandler;

public class When_room_has_items_and_exits
{
    private readonly Questline.Engine.Handlers.GetRoomDetailsHandler _handler;

    public When_room_has_items_and_exits()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Hallway
                .WithItem(Items.BrassLamp)
                .WithExit(Direction.North, "throne-room")
                .WithExit(Direction.South, "entrance"))
            .WithRoom(Rooms.ThroneRoom)
            .WithRoom(Rooms.Entrance)
            .Build("hallway");

        _handler = new Questline.Engine.Handlers.GetRoomDetailsHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
    }

    [Fact]
    public async Task Returns_response_with_room_details()
    {
        var result = await _handler.Handle(new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.RoomName.ShouldBe("Hallway");
        lookResult.Description.ShouldBe("A long hallway.");
        lookResult.Items.ShouldContain("brass lamp");
        lookResult.Exits.ShouldContain("North");
        lookResult.Exits.ShouldContain("South");
    }
}
