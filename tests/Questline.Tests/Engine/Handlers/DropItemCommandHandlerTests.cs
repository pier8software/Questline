using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class DropItemCommandHandlerTests
{
    [Fact]
    public async Task Returns_successful_drop_response()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var fixture = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .WithInventoryItem(lamp)
            .Build("cellar");

        var handler = new DropItemCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.DropItemCommand("brass lamp"));

        var dropResult = result.ShouldBeOfType<Responses.ItemDroppedResponse>();
        dropResult.ItemName.ShouldBe("brass lamp");
    }

    [Fact]
    public async Task Item_moves_from_inventory_to_room()
    {
        var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
        var fixture = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .WithInventoryItem(lamp)
            .Build("cellar");

        var handler = new DropItemCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        _ = await handler.Handle(new Requests.DropItemCommand("brass lamp"));

        fixture.Playthrough.Inventory.ShouldBeEmpty();
        var recordedItems = fixture.Playthrough.GetRecordedRoomItems("cellar");
        recordedItems.ShouldNotBeNull();
        recordedItems!.ShouldContain(i => i.Name == "brass lamp");
    }

    [Fact]
    public async Task Item_not_in_inventory_returns_error()
    {
        var fixture = new GameBuilder()
            .WithRoom("cellar", "Cellar", "A damp cellar.")
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
            .WithRoom("cellar", "Cellar", "A damp cellar.")
            .Build("cellar");

        var handler = new DropItemCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.DropItemCommand("BRASS LAMP"));

        var dropResult = result.ShouldBeOfType<Responses.ItemDroppedResponse>();
        dropResult.ItemName.ShouldContain("BRASS LAMP");
    }
}
