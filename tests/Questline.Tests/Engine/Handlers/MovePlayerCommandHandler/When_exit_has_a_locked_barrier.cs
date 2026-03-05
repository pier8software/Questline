using Questline.Domain.Rooms.Entity;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.MovePlayerCommandHandler;

public class When_exit_has_a_locked_barrier
{
    private readonly Questline.Engine.Handlers.MovePlayerCommandHandler _handler;
    private readonly GameFixture                                        _fixture;

    public When_exit_has_a_locked_barrier()
    {
        _fixture = new GameBuilder()
            .WithRoom(Rooms.StartRoom
                .WithExit(Direction.North, Exits.WithBarrier.WithDestination("end")))
            .WithRoom(Rooms.EndRoom)
            .Build("start");

        _handler = new Questline.Engine.Handlers.MovePlayerCommandHandler(
            _fixture.Session, _fixture.PlaythroughRepository, _fixture.RoomRepository);
    }

    [Fact]
    public async Task Blocked_exit_returns_barrier_message()
    {
        var result = await _handler.Handle(new Requests.MovePlayerCommand(Direction.North));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("The iron door is locked tight.");
        _fixture.Playthrough.Location.ShouldBe("start");
    }
}
