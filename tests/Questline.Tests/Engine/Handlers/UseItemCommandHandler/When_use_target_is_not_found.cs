using Questline.Domain.Rooms.Entity;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.UseItemCommandHandler;

public class When_use_target_is_not_found
{
    private readonly Questline.Engine.Handlers.UseItemCommandHandler _handler;

    public When_use_target_is_not_found()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber.WithExit(Direction.North, "beyond"))
            .WithRoom(Rooms.BeyondRoom)
            .WithInventoryItem(Items.RustyKey)
            .Build("chamber");

        _handler = new Questline.Engine.Handlers.UseItemCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
    }

    [Fact]
    public async Task Returns_error_message()
    {
        var result = await _handler.Handle(new PartyActor(), new Requests.UseItemCommand("rusty key", "iron door"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("You don't see 'iron door' here.");
    }
}
