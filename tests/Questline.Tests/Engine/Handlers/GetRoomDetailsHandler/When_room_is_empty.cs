using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class When_room_is_empty
{
    private readonly GetRoomDetailsHandler _handler;

    public When_room_is_empty()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar)
            .Build("cellar");

        _handler = new GetRoomDetailsHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
    }

    [Fact]
    public async Task Response_omits_items_when_room_is_empty()
    {
        var result = await _handler.Handle(new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.Items.ShouldBeEmpty();
    }
}
