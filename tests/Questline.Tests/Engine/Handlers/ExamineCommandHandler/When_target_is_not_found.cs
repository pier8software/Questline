using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class When_target_is_not_found
{
    private readonly ExamineCommandHandler _handler;

    public When_target_is_not_found()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber)
            .Build("chamber");

        _handler = new ExamineCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
    }

    [Fact]
    public async Task Returns_error_message()
    {
        var result = await _handler.Handle(new Requests.ExamineCommand("mysterious orb"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("You don't see 'mysterious orb' here.");
    }
}
