using Questline.Domain.Rooms.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class UseItemCommandHandlerTests
{
    [Fact]
    public async Task Correct_item_on_barrier_unlocks_it()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber
                .WithExit(Direction.North, Exits.WithBarrier.WithDestination("beyond")))
            .WithRoom(Rooms.BeyondRoom)
            .WithInventoryItem(Items.RustyKey)
            .Build("chamber");

        var handler = new UseItemCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.UseItemCommand("rusty key", "iron door"));

        var useResult = result.ShouldBeOfType<Responses.UseItemResponse>();
        useResult.ResultMessage.ShouldBe("The rusty key turns in the lock and the iron door swings open.");
        fixture.Playthrough.IsBarrierUnlocked("iron-door").ShouldBeTrue();
    }

    [Fact]
    public async Task Wrong_item_returns_error_and_barrier_stays_locked()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber
                .WithExit(Direction.North, Exits.WithBarrier.WithDestination("beyond")))
            .WithRoom(Rooms.BeyondRoom)
            .WithInventoryItem(Items.Torch)
            .Build("chamber");

        var handler = new UseItemCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.UseItemCommand("torch", "iron door"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("The torch doesn't work on the iron door.");
        fixture.Playthrough.IsBarrierUnlocked("iron-door").ShouldBeFalse();
    }

    [Fact]
    public async Task Item_not_in_inventory_returns_error()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber
                .WithExit(Direction.North, Exits.WithBarrier.WithDestination("beyond")))
            .WithRoom(Rooms.BeyondRoom)
            .Build("chamber");

        var handler = new UseItemCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.UseItemCommand("rusty key", "iron door"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("You don't have 'rusty key'.");
    }

    [Fact]
    public async Task Contextual_use_unlocks_matching_barrier_in_room()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber
                .WithExit(Direction.North, Exits.WithBarrier.WithDestination("beyond")))
            .WithRoom(Rooms.BeyondRoom)
            .WithInventoryItem(Items.RustyKey)
            .Build("chamber");

        var handler = new UseItemCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.UseItemCommand("rusty key", null));

        var useResult = result.ShouldBeOfType<Responses.UseItemResponse>();
        useResult.ResultMessage.ShouldBe("The rusty key turns in the lock and the iron door swings open.");
        fixture.Playthrough.IsBarrierUnlocked("iron-door").ShouldBeTrue();
    }

    [Fact]
    public async Task Target_not_found_returns_error()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber.WithExit(Direction.North, "beyond"))
            .WithRoom(Rooms.BeyondRoom)
            .WithInventoryItem(Items.RustyKey)
            .Build("chamber");

        var handler = new UseItemCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.UseItemCommand("rusty key", "iron door"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("You don't see 'iron door' here.");
    }

    [Fact]
    public async Task Already_unlocked_barrier_returns_informative_message()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber
                .WithExit(Direction.North, Exits.WithBarrier.WithDestination("beyond")))
            .WithRoom(Rooms.BeyondRoom)
            .WithInventoryItem(Items.RustyKey)
            .WithUnlockedBarrier("iron-door")
            .Build("chamber");

        var handler = new UseItemCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.UseItemCommand("rusty key", "iron door"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("The iron door is already unlocked.");
    }
}
