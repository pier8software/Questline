using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.GetRoomDetailsHandler;

public class When_room_is_empty
{
    private readonly Questline.Engine.Handlers.GetRoomDetailsHandler _handler;

    public When_room_is_empty()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar)
            .Build("cellar");

        _handler = new Questline.Engine.Handlers.GetRoomDetailsHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
    }

    [Fact]
    public async Task Response_omits_items_when_room_is_empty()
    {
        var result = await _handler.Handle(new PartyActor(), new Requests.GetRoomDetailsQuery());

        var lookResult = result.ShouldBeOfType<Responses.RoomDetailsResponse>();
        lookResult.Items.ShouldBeEmpty();
    }
}
