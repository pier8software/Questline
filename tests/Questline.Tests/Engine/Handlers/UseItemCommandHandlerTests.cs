using Questline.Domain.Rooms.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class UseItemCommandHandlerTests
{
    public class When_using_correct_item_on_barrier
    {
        private readonly UseItemCommandHandler _handler;
        private readonly GameFixture _fixture;

        public When_using_correct_item_on_barrier()
        {
            _fixture = new GameBuilder()
                .WithRoom(Rooms.Chamber
                    .WithExit(Direction.North, Exits.WithBarrier.WithDestination("beyond")))
                .WithRoom(Rooms.BeyondRoom)
                .WithInventoryItem(Items.RustyKey)
                .Build("chamber");

            _handler = new UseItemCommandHandler(
                _fixture.Session, _fixture.PlaythroughRepository, _fixture.RoomRepository);
        }

        [Fact]
        public async Task Barrier_is_unlocked_with_message()
        {
            var result = await _handler.Handle(new Requests.UseItemCommand("rusty key", "iron door"));

            var useResult = result.ShouldBeOfType<Responses.UseItemResponse>();
            useResult.ResultMessage.ShouldBe("The rusty key turns in the lock and the iron door swings open.");
            _fixture.Playthrough.IsBarrierUnlocked("iron-door").ShouldBeTrue();
        }

        [Fact]
        public async Task Contextual_use_unlocks_matching_barrier_in_room()
        {
            var result = await _handler.Handle(new Requests.UseItemCommand("rusty key", null));

            var useResult = result.ShouldBeOfType<Responses.UseItemResponse>();
            useResult.ResultMessage.ShouldBe("The rusty key turns in the lock and the iron door swings open.");
            _fixture.Playthrough.IsBarrierUnlocked("iron-door").ShouldBeTrue();
        }
    }

    public class When_using_wrong_item_on_barrier
    {
        [Fact]
        public async Task Returns_error_and_barrier_stays_locked()
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
    }

    public class When_item_is_not_in_inventory
    {
        [Fact]
        public async Task Returns_error_message()
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
    }

    public class When_target_is_not_found
    {
        [Fact]
        public async Task Returns_error_message()
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
    }

    public class When_barrier_is_already_unlocked
    {
        [Fact]
        public async Task Returns_informative_message()
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
}
