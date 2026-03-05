using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.ExamineCommandHandler;

public class When_examining_inventory_item
{
    private readonly Questline.Engine.Handlers.ExamineCommandHandler _handler;

    public When_examining_inventory_item()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber)
            .WithInventoryItem(Items.RustyKey)
            .Build("chamber");

        _handler = new Questline.Engine.Handlers.ExamineCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
    }

    [Fact]
    public async Task Shows_item_description()
    {
        var result = await _handler.Handle(new Requests.ExamineCommand("rusty key"));

        var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
        examineResult.Description.ShouldBe("An old iron key, its teeth worn by time.");
    }
}
