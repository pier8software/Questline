using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class When_taking_and_dropping_items
{
    private readonly TakeItemHandler _takeHandler;
    private readonly DropItemCommandHandler _dropHandler;
    private readonly GameFixture _fixture;

    public When_taking_and_dropping_items()
    {
        _fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar.WithItem(Items.BrassLamp))
            .Build("cellar");

        _takeHandler = new TakeItemHandler(
            _fixture.Session, _fixture.PlaythroughRepository, _fixture.RoomRepository);
        _dropHandler = new DropItemCommandHandler(
            _fixture.Session, _fixture.PlaythroughRepository, _fixture.RoomRepository);
    }

    [Fact]
    public async Task Get_then_drop_round_trips_item_through_inventory()
    {
        await _takeHandler.Handle(new Requests.TakeItemCommand("brass lamp"));
        _fixture.Playthrough.Inventory.ShouldContain(i => i.Name == "brass lamp");

        await _dropHandler.Handle(new Requests.DropItemCommand("brass lamp"));
        _fixture.Playthrough.Inventory.ShouldBeEmpty();
        var recordedItems = _fixture.Playthrough.GetRecordedRoomItems("cellar");
        recordedItems.ShouldNotBeNull();
        recordedItems.ShouldContain(i => i.Name == "brass lamp");
    }
}
