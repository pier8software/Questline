using Questline.Domain.Rooms.Entity;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.UseItemCommandHandler;

public class When_using_correct_item_on_barrier
{
    private readonly Questline.Engine.Handlers.UseItemCommandHandler _handler;
    private readonly GameFixture                                     _fixture;

    public When_using_correct_item_on_barrier()
    {
        _fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber
                .WithExit(Direction.North, Exits.WithBarrier.WithDestination("beyond")))
            .WithRoom(Rooms.BeyondRoom)
            .WithInventoryItem(Items.RustyKey)
            .Build("chamber");

        _handler = new Questline.Engine.Handlers.UseItemCommandHandler(
            _fixture.Session, _fixture.PlaythroughRepository, _fixture.RoomRepository);
    }

    [Fact]
    public async Task Barrier_is_unlocked_with_message()
    {
        var result = await _handler.Handle(new PartyActor(), new Requests.UseItemCommand("rusty key", "iron door"));

        var useResult = result.ShouldBeOfType<Responses.UseItemResponse>();
        useResult.ResultMessage.ShouldBe("The rusty key turns in the lock and the iron door swings open.");
        _fixture.Playthrough.IsBarrierUnlocked("iron-door").ShouldBeTrue();
    }

    [Fact]
    public async Task Contextual_use_unlocks_matching_barrier_in_room()
    {
        var result = await _handler.Handle(new PartyActor(), new Requests.UseItemCommand("rusty key", null));

        var useResult = result.ShouldBeOfType<Responses.UseItemResponse>();
        useResult.ResultMessage.ShouldBe("The rusty key turns in the lock and the iron door swings open.");
        _fixture.Playthrough.IsBarrierUnlocked("iron-door").ShouldBeTrue();
    }
}
