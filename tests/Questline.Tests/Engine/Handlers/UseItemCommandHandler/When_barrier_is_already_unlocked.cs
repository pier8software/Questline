using Questline.Domain.Rooms.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class When_barrier_is_already_unlocked
{
    private readonly UseItemCommandHandler _handler;

    public When_barrier_is_already_unlocked()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber
                .WithExit(Direction.North, Exits.WithBarrier.WithDestination("beyond")))
            .WithRoom(Rooms.BeyondRoom)
            .WithInventoryItem(Items.RustyKey)
            .WithUnlockedBarrier("iron-door")
            .Build("chamber");

        _handler = new UseItemCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
    }

    [Fact]
    public async Task Returns_informative_message()
    {
        var result = await _handler.Handle(new Requests.UseItemCommand("rusty key", "iron door"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("The iron door is already unlocked.");
    }
}
