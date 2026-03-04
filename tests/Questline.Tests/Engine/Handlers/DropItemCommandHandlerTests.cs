using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class DropItemCommandHandlerTests
{
    private readonly DropItemCommandHandler _handler;
    private readonly GameFixture _fixture;

    public DropItemCommandHandlerTests()
    {
        _fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar)
            .WithInventoryItem(Items.BrassLamp)
            .Build("cellar");

        _handler = new DropItemCommandHandler(
            _fixture.Session, _fixture.PlaythroughRepository, _fixture.RoomRepository);
    }

    [Fact]
    public async Task Returns_successful_drop_response()
    {
        var result = await _handler.Handle(new Requests.DropItemCommand("brass lamp"));

        var dropResult = result.ShouldBeOfType<Responses.ItemDroppedResponse>();
        dropResult.ItemName.ShouldBe("brass lamp");
    }

    [Fact]
    public async Task Item_moves_from_inventory_to_room()
    {
        _ = await _handler.Handle(new Requests.DropItemCommand("brass lamp"));

        _fixture.Playthrough.Inventory.ShouldBeEmpty();
        var recordedItems = _fixture.Playthrough.GetRecordedRoomItems("cellar");
        recordedItems.ShouldNotBeNull();
        recordedItems!.ShouldContain(i => i.Name == "brass lamp");
    }

    public class When_item_is_not_in_inventory
    {
        [Fact]
        public async Task Returns_error_message()
        {
            var fixture = new GameBuilder()
                .WithRoom(Rooms.Cellar)
                .Build("cellar");

            var handler = new DropItemCommandHandler(
                fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

            var result = await handler.Handle(new Requests.DropItemCommand("lamp"));

            var dropResult = result.ShouldBeOfType<Responses.ItemDroppedResponse>();
            dropResult.ItemName.ShouldContain("You are not carrying 'lamp'.");
        }

        [Fact]
        public async Task Matching_is_case_insensitive()
        {
            var fixture = new GameBuilder()
                .WithRoom(Rooms.Cellar)
                .Build("cellar");

            var handler = new DropItemCommandHandler(
                fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

            var result = await handler.Handle(new Requests.DropItemCommand("BRASS LAMP"));

            var dropResult = result.ShouldBeOfType<Responses.ItemDroppedResponse>();
            dropResult.ItemName.ShouldContain("BRASS LAMP");
        }
    }
}
