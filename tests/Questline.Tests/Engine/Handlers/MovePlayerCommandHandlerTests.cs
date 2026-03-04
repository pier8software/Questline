using Questline.Domain.Rooms.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class MovePlayerCommandHandlerTests
{
    public class When_exit_is_open
    {
        private readonly MovePlayerCommandHandler _handler;
        private readonly GameFixture _fixture;

        public When_exit_is_open()
        {
            _fixture = new GameBuilder()
                .WithRoom(Rooms.StartRoom.WithExit(Direction.North, "end"))
                .WithRoom(Rooms.EndRoom.WithExit(Direction.South, "start"))
                .Build("start");

            _handler = new MovePlayerCommandHandler(
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

    public class When_room_has_no_exits
    {
        [Fact]
        public async Task Player_location_is_not_updated()
        {
            var fixture = new GameBuilder()
                .WithRoom(Rooms.SealedRoom)
                .Build("sealed");

            var handler = new MovePlayerCommandHandler(
                fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

            _ = await handler.Handle(new Requests.MovePlayerCommand(Direction.North));

            fixture.Playthrough.Location.ShouldBe("sealed");
        }
    }

    public class When_exit_has_a_locked_barrier
    {
        private readonly MovePlayerCommandHandler _handler;
        private readonly GameFixture _fixture;

        public When_exit_has_a_locked_barrier()
        {
            _fixture = new GameBuilder()
                .WithRoom(Rooms.StartRoom
                    .WithExit(Direction.North, Exits.WithBarrier.WithDestination("end")))
                .WithRoom(Rooms.EndRoom)
                .Build("start");

            _handler = new MovePlayerCommandHandler(
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

    public class When_barrier_is_unlocked
    {
        [Fact]
        public async Task Player_moves_through_unlocked_barrier()
        {
            var fixture = new GameBuilder()
                .WithRoom(Rooms.StartRoom
                    .WithExit(Direction.North, Exits.WithBarrier.WithDestination("end")))
                .WithRoom(Rooms.EndRoom.WithExit(Direction.South, Exits.Default.WithDestination("start")))
                .WithUnlockedBarrier("iron-door")
                .Build("start");

            var handler = new MovePlayerCommandHandler(
                fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

            var result = await handler.Handle(new Requests.MovePlayerCommand(Direction.North));

            var moveResult = result.ShouldBeOfType<Responses.PlayerMovedResponse>();
            moveResult.RoomName.ShouldBe("End Room");
            fixture.Playthrough.Location.ShouldBe("end");
        }
    }
}
