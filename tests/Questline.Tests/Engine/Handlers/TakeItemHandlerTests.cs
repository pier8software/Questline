using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class TakeItemHandlerTests
{
    [Fact]
    public async Task Returns_successful_take_response()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar.WithItem(Items.BrassLamp))
            .Build("cellar");

        var handler = new TakeItemHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.TakeItemCommand("brass lamp"));

        var takeResult = result.ShouldBeOfType<Responses.ItemTakenResponse>();
        takeResult.ItemName.ShouldBe("brass lamp");
    }

    [Fact]
    public async Task Item_moves_from_room_to_inventory()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar.WithItem(Items.BrassLamp.Build()))
            .Build("cellar");

        var handler = new TakeItemHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        _ = await handler.Handle(new Requests.TakeItemCommand("brass lamp"));

        fixture.Playthrough.Inventory.ShouldContain(Items.BrassLamp.Build());
        var recordedItems = fixture.Playthrough.GetRecordedRoomItems("cellar");
        recordedItems.ShouldNotBeNull();
        recordedItems.ShouldNotContain(i => i.Name == "brass lamp");
    }

    [Fact]
    public async Task Item_not_in_room_returns_error_message()
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

    [Fact]
    public async Task Matching_is_case_insensitive()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar.WithItem(Items.BrassLamp))
            .Build("cellar");

        var handler = new TakeItemHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.TakeItemCommand("BRASS LAMP"));

        var takeResult = result.ShouldBeOfType<Responses.ItemTakenResponse>();
        takeResult.ItemName.ShouldBe("brass lamp");
    }
}
