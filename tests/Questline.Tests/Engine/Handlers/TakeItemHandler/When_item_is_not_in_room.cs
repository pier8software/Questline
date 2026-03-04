using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class When_item_is_not_in_room
{
    private readonly TakeItemHandler _handler;

    public When_item_is_not_in_room()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar)
            .Build("cellar");

        _handler = new TakeItemHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
    }

    [Fact]
    public async Task Returns_error_message()
    {
        var result = await _handler.Handle(new Requests.TakeItemCommand("lamp"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldContain("There is no 'lamp' here.");
    }
}
