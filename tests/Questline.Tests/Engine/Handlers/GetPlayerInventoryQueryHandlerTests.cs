using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class GetPlayerInventoryQueryHandlerTests
{
    [Fact]
    public async Task Lists_carried_items()
    {
        var lamp = Items.BrassLamp.Build();
        var key  = Items.RustyKey.Build();
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar)
            .WithInventoryItem(lamp)
            .WithInventoryItem(key)
            .Build("cellar");

        var handler = new GetPlayerInventoryQueryHandler(
            fixture.Session, fixture.PlaythroughRepository);

        var result = await handler.Handle(new Requests.GetPlayerInventoryQuery());

        var inventoryResult = result.ShouldBeOfType<Responses.PlayerInventoryResponse>();
        inventoryResult.Items.ShouldContain("brass lamp");
        inventoryResult.Items.ShouldContain("rusty key");
    }

    [Fact]
    public async Task Empty_inventory_returns_empty_items_list()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar)
            .Build("cellar");

        var handler = new GetPlayerInventoryQueryHandler(
            fixture.Session, fixture.PlaythroughRepository);

        var result = await handler.Handle(new Requests.GetPlayerInventoryQuery());

        var inventoryResult = result.ShouldBeOfType<Responses.PlayerInventoryResponse>();
        inventoryResult.Items.ShouldBeEmpty();
    }

    [Fact]
    public async Task Get_then_drop_round_trips_item_through_inventory()
    {
        var lamp = Items.BrassLamp.Build();
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar.WithItem(lamp))
            .Build("cellar");

        var takeHandler = new TakeItemHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
        var dropHandler = new DropItemCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        await takeHandler.Handle(new Requests.TakeItemCommand("brass lamp"));
        fixture.Playthrough.Inventory.ShouldContain(lamp);

        await dropHandler.Handle(new Requests.DropItemCommand("brass lamp"));
        fixture.Playthrough.Inventory.ShouldBeEmpty();
        var recordedItems = fixture.Playthrough.GetRecordedRoomItems("cellar");
        recordedItems.ShouldNotBeNull();
        recordedItems!.ShouldContain(i => i.Name == "brass lamp");
    }
}
