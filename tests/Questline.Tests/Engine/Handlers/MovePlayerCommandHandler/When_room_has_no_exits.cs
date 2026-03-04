using Questline.Domain.Rooms.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class When_room_has_no_exits
{
    private readonly MovePlayerCommandHandler _handler;
    private readonly GameFixture _fixture;

    public When_room_has_no_exits()
    {
        _fixture = new GameBuilder()
            .WithRoom(Rooms.SealedRoom)
            .Build("sealed");

        _handler = new MovePlayerCommandHandler(
            _fixture.Session, _fixture.PlaythroughRepository, _fixture.RoomRepository);
    }

    [Fact]
    public async Task Player_location_is_not_updated()
    {
        _ = await _handler.Handle(new Requests.MovePlayerCommand(Direction.North));

        _fixture.Playthrough.Location.ShouldBe("sealed");
    }
}
