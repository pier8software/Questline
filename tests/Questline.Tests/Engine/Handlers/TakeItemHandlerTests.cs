using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class TakeItemHandlerTests
{
    private readonly TakeItemHandler _handler;
    private readonly GameFixture _fixture;

    public TakeItemHandlerTests()
    {
        _fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar.WithItem(Items.BrassLamp))
            .Build("cellar");

        _handler = new TakeItemHandler(
            _fixture.Session, _fixture.PlaythroughRepository, _fixture.RoomRepository);
    }

    [Fact]
    public async Task Returns_successful_take_response()
    {
        var result = await _handler.Handle(new Requests.TakeItemCommand("brass lamp"));

        var takeResult = result.ShouldBeOfType<Responses.ItemTakenResponse>();
        takeResult.ItemName.ShouldBe("brass lamp");
    }

    [Fact]
    public async Task Matching_is_case_insensitive()
    {
        var result = await _handler.Handle(new Requests.TakeItemCommand("BRASS LAMP"));

        var takeResult = result.ShouldBeOfType<Responses.ItemTakenResponse>();
        takeResult.ItemName.ShouldBe("brass lamp");
    }

    public class When_item_is_not_in_room
    {
        [Fact]
        public async Task Returns_error_message()
        {
            var fixture = new GameBuilder()
                .WithRoom(Rooms.Cellar)
                .Build("cellar");

            var handler = new TakeItemHandler(
                fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

            var result = await handler.Handle(new Requests.TakeItemCommand("lamp"));

            var error = result.ShouldBeOfType<ErrorResponse>();
            error.ErrorMessage.ShouldContain("There is no 'lamp' here.");
        }
    }
}
