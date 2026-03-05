using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.DropItemCommandHandler;

public class When_item_is_in_inventory
{
    private readonly Questline.Engine.Handlers.DropItemCommandHandler _handler;
    private readonly GameFixture                                      _fixture;

    public When_item_is_in_inventory()
    {
        _fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar)
            .WithInventoryItem(Items.BrassLamp)
            .Build("cellar");

        _handler = new Questline.Engine.Handlers.DropItemCommandHandler(
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
}
