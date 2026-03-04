using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class When_item_is_in_room
{
    private readonly TakeItemHandler _handler;
    private readonly GameFixture _fixture;

    public When_item_is_in_room()
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
}
