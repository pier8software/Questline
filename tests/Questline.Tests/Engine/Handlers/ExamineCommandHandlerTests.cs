using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class ExamineCommandHandlerTests
{
    public class When_examining_inventory_item
    {
        private readonly ExamineCommandHandler _handler;

        public When_examining_inventory_item()
        {
            var fixture = new GameBuilder()
                .WithRoom(Rooms.Chamber)
                .WithInventoryItem(Items.RustyKey)
                .Build("chamber");

            _handler = new ExamineCommandHandler(
                fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
        }

        [Fact]
        public async Task Shows_item_description()
        {
            var result = await _handler.Handle(new Requests.ExamineCommand("rusty key"));

            var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
            examineResult.Description.ShouldBe("An old iron key, its teeth worn by time.");
        }
    }

    public class When_examining_room_item
    {
        private readonly ExamineCommandHandler _handler;

        public When_examining_room_item()
        {
            var fixture = new GameBuilder()
                .WithRoom(Rooms.Chamber.WithItem(Items.Torch))
                .Build("chamber");

            _handler = new ExamineCommandHandler(
                fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
        }

        [Fact]
        public async Task Shows_item_description()
        {
            var result = await _handler.Handle(new Requests.ExamineCommand("torch"));

            var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
            examineResult.Description.ShouldBe("A flickering wooden torch.");
        }
    }

    public class When_examining_room_feature
    {
        private readonly ExamineCommandHandler _handler;

        public When_examining_room_feature()
        {
            var fixture = new GameBuilder()
                .WithRoom(Rooms.Chamber.WithFeature(Features.StrangeSymbols))
                .Build("chamber");

            _handler = new ExamineCommandHandler(
                fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
        }

        [Fact]
        public async Task Keyword_shows_description()
        {
            var result = await _handler.Handle(new Requests.ExamineCommand("symbols"));

            var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
            examineResult.Description.ShouldBe("Ancient runes etched into the stone walls.");
        }

        [Fact]
        public async Task Full_name_shows_description()
        {
            var result = await _handler.Handle(new Requests.ExamineCommand("strange symbols"));

            var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
            examineResult.Description.ShouldBe("Ancient runes etched into the stone walls.");
        }
    }

    public class When_target_is_not_found
    {
        [Fact]
        public async Task Returns_error_message()
        {
            var fixture = new GameBuilder()
                .WithRoom(Rooms.Chamber)
                .Build("chamber");

            var handler = new ExamineCommandHandler(
                fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

            var result = await handler.Handle(new Requests.ExamineCommand("mysterious orb"));

            var error = result.ShouldBeOfType<ErrorResponse>();
            error.ErrorMessage.ShouldBe("You don't see 'mysterious orb' here.");
        }
    }
}
