using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class GetPlayerInventoryQueryHandlerTests
{
    [Fact]
    public async Task Lists_carried_items()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var key  = new Item { Id = "key", Name  = "rusty key", Description  = "A rusty iron key." };
        var fixture = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
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
            .WithRoom("cellar", "Cellar", "A damp cellar.")
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
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var fixture = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
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
