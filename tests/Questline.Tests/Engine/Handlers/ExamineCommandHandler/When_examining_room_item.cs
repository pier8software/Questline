using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class When_examining_room_item
{
    private readonly ExamineCommandHandler _handler;

    public When_examining_room_item()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber.WithItem(Items.Torch))
            .Build("chamber");

        _handler = new ExamineCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
    }

    [Fact]
    public async Task Shows_item_description()
    {
        var result = await _handler.Handle(new Requests.ExamineCommand("torch"));

        var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
        examineResult.Description.ShouldBe("A flickering wooden torch.");
    }
}
