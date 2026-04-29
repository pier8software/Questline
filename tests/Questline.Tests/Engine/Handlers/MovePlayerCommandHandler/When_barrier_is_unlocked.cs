using Questline.Domain.Rooms.Entity;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.MovePlayerCommandHandler;

public class When_barrier_is_unlocked
{
    private readonly Questline.Engine.Handlers.MovePlayerCommandHandler _handler;
    private readonly GameFixture                                        _fixture;

    public When_barrier_is_unlocked()
    {
        _fixture = new GameBuilder()
            .WithRoom(Rooms.StartRoom
                .WithExit(Direction.North, Exits.WithBarrier.WithDestination("end")))
            .WithRoom(Rooms.EndRoom.WithExit(Direction.South, Exits.Default.WithDestination("start")))
            .WithUnlockedBarrier("iron-door")
            .Build("start");

        _handler = new Questline.Engine.Handlers.MovePlayerCommandHandler(
            _fixture.Session, _fixture.PlaythroughRepository, _fixture.RoomRepository);
    }

    [Fact]
    public async Task Player_moves_through_unlocked_barrier()
    {
        var result = await _handler.Handle(new PartyActor(), new Requests.MovePlayerCommand(Direction.North));

        var moveResult = result.ShouldBeOfType<Responses.PlayerMovedResponse>();
        moveResult.RoomName.ShouldBe("End Room");
        _fixture.Playthrough.Location.ShouldBe("end");
    }
}
