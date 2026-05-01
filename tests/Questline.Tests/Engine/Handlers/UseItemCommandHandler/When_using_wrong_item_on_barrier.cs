using Questline.Domain.Rooms.Entity;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.UseItemCommandHandler;

public class When_using_wrong_item_on_barrier
{
    private readonly Questline.Engine.Handlers.UseItemCommandHandler _handler;
    private readonly GameFixture                                     _fixture;

    public When_using_wrong_item_on_barrier()
    {
        _fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber
                .WithExit(Direction.North, Exits.WithBarrier.WithDestination("beyond")))
            .WithRoom(Rooms.BeyondRoom)
            .WithInventoryItem(Items.Torch)
            .Build("chamber");

        _handler = new Questline.Engine.Handlers.UseItemCommandHandler(
            _fixture.Session, _fixture.PlaythroughRepository, _fixture.RoomRepository);
    }

    [Fact]
    public async Task Returns_error_and_barrier_stays_locked()
    {
        var result = await _handler.Handle(new PartyActor(), new Requests.UseItemCommand("torch", "iron door"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("The torch doesn't work on the iron door.");
        _fixture.Playthrough.IsBarrierUnlocked("iron-door").ShouldBeFalse();
    }
}
