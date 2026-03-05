using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.DropItemCommandHandler;

public class When_item_is_not_in_inventory
{
    private readonly Questline.Engine.Handlers.DropItemCommandHandler _handler;
    private readonly GameFixture                                      _fixture;

    public When_item_is_not_in_inventory()
    {
        _fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar)
            .Build("cellar");

        _handler = new Questline.Engine.Handlers.DropItemCommandHandler(
            _fixture.Session, _fixture.PlaythroughRepository, _fixture.RoomRepository);
    }

    [Fact]
    public async Task Returns_error_message()
    {
        var result = await _handler.Handle(new Requests.DropItemCommand("lamp"));

        var dropResult = result.ShouldBeOfType<Responses.ItemDroppedResponse>();
        dropResult.ItemName.ShouldContain("You are not carrying 'lamp'.");
    }

    [Fact]
    public async Task Matching_is_case_insensitive()
    {
        var result = await _handler.Handle(new Requests.DropItemCommand("BRASS LAMP"));

        var dropResult = result.ShouldBeOfType<Responses.ItemDroppedResponse>();
        dropResult.ItemName.ShouldContain("BRASS LAMP");
    }
}
