using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class When_player_has_items
{
    private readonly GetPlayerInventoryQueryHandler _handler;

    public When_player_has_items()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar)
            .WithInventoryItem(Items.BrassLamp)
            .WithInventoryItem(Items.RustyKey)
            .Build("cellar");

        _handler = new GetPlayerInventoryQueryHandler(
            fixture.Session, fixture.PlaythroughRepository);
    }

    [Fact]
    public async Task Lists_carried_items()
    {
        var result = await _handler.Handle(new Requests.GetPlayerInventoryQuery());

        var inventoryResult = result.ShouldBeOfType<Responses.PlayerInventoryResponse>();
        inventoryResult.Items.ShouldContain("brass lamp");
        inventoryResult.Items.ShouldContain("rusty key");
    }
}
