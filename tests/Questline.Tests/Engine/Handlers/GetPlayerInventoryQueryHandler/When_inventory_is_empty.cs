using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class When_inventory_is_empty
{
    private readonly GetPlayerInventoryQueryHandler _handler;

    public When_inventory_is_empty()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar)
            .Build("cellar");

        _handler = new GetPlayerInventoryQueryHandler(
            fixture.Session, fixture.PlaythroughRepository);
    }

    [Fact]
    public async Task Returns_empty_items_list()
    {
        var result = await _handler.Handle(new Requests.GetPlayerInventoryQuery());

        var inventoryResult = result.ShouldBeOfType<Responses.PlayerInventoryResponse>();
        inventoryResult.Items.ShouldBeEmpty();
    }
}
