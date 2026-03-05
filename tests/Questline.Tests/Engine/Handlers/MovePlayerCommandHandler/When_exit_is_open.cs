using Questline.Domain.Rooms.Entity;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.MovePlayerCommandHandler;

public class When_exit_is_open
{
    private readonly Questline.Engine.Handlers.MovePlayerCommandHandler _handler;
    private readonly GameFixture                                        _fixture;

    public When_exit_is_open()
    {
        _fixture = new GameBuilder()
            .WithRoom(Rooms.StartRoom.WithExit(Direction.North, "end"))
            .WithRoom(Rooms.EndRoom.WithExit(Direction.South, "start"))
            .Build("start");

        _handler = new Questline.Engine.Handlers.MovePlayerCommandHandler(
            _fixture.Session, _fixture.PlaythroughRepository, _fixture.RoomRepository);
    }

    [Fact]
    public async Task Returns_next_room_details_in_response()
    {
        var result = await _handler.Handle(new Requests.MovePlayerCommand(Direction.North));

        var moveResult = result.ShouldBeOfType<Responses.PlayerMovedResponse>();
        moveResult.RoomName.ShouldBe("End Room");
        moveResult.Description.ShouldBe("The end room.");
    }

    [Fact]
    public async Task Invalid_direction_returns_error_message()
    {
        var result = await _handler.Handle(new Requests.MovePlayerCommand(Direction.East));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("There is no exit to the East.");
    }

    [Fact]
    public async Task Player_location_is_updated_after_moving()
    {
        _ = await _handler.Handle(new Requests.MovePlayerCommand(Direction.North));

        _fixture.Playthrough.Location.ShouldBe("end");
    }
}
