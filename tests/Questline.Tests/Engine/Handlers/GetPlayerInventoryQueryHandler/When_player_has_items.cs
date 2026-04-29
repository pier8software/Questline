using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.GetPlayerInventoryQueryHandler;

public class When_player_has_items
{
    private readonly Questline.Engine.Handlers.GetPlayerInventoryQueryHandler _handler;

    public When_player_has_items()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Cellar)
            .WithInventoryItem(Items.BrassLamp)
            .WithInventoryItem(Items.RustyKey)
            .Build("cellar");

        _handler = new Questline.Engine.Handlers.GetPlayerInventoryQueryHandler(
            fixture.Session, fixture.PlaythroughRepository);
    }

    [Fact]
    public async Task Lists_carried_items()
    {
        var result = await _handler.Handle(new PartyActor(), new Requests.GetPlayerInventoryQuery());

        var inventoryResult = result.ShouldBeOfType<Responses.PlayerInventoryResponse>();
        inventoryResult.Items.ShouldContain("brass lamp");
        inventoryResult.Items.ShouldContain("rusty key");
    }
}
