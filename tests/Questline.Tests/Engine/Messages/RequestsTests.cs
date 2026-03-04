using Questline.Domain.Rooms.Entity;
using Questline.Engine.Messages;

namespace Questline.Tests.Engine.Messages;

public class RequestsTests
{
    [Fact]
    public void DropItemCommand_is_created_from_args()
    {
        var request = Requests.DropItemCommand.CreateRequest(["sword"]);

        var command = request.ShouldBeOfType<Requests.DropItemCommand>();
        command.ItemName.ShouldBe("sword");
    }

    [Fact]
    public void ExamineCommand_is_created_from_args()
    {
        var request = Requests.ExamineCommand.CreateRequest(["sword"]);

        var command = request.ShouldBeOfType<Requests.ExamineCommand>();
        command.TargetName.ShouldBe("sword");
    }

    [Fact]
    public void GetRoomDetailsQuery_is_created_from_empty_args()
    {
        var request = Requests.GetRoomDetailsQuery.CreateRequest([]);

        request.ShouldBeOfType<Requests.GetRoomDetailsQuery>();
    }

    [Fact]
    public void GetPlayerInventoryQuery_is_created_from_empty_args()
    {
        var request = Requests.GetPlayerInventoryQuery.CreateRequest([]);

        request.ShouldBeOfType<Requests.GetPlayerInventoryQuery>();
    }

    [Theory]
    [InlineData("north", Direction.North)]
    [InlineData("south", Direction.South)]
    [InlineData("east",  Direction.East)]
    [InlineData("west",  Direction.West)]
    [InlineData("up",    Direction.Up)]
    [InlineData("down",  Direction.Down)]
    [InlineData("left",  Direction.Invalid)]
    public void MovePlayerCommand_parses_direction_from_args(string direction, Direction expectedDirection)
    {
        var request = Requests.MovePlayerCommand.CreateRequest([direction]);

        var command = request.ShouldBeOfType<Requests.MovePlayerCommand>();
        command.Direction.ShouldBe(expectedDirection);
    }

    [Fact]
    public void TakeItemCommand_is_created_from_args()
    {
        var request = Requests.TakeItemCommand.CreateRequest(["sword"]);

        var command = request.ShouldBeOfType<Requests.TakeItemCommand>();
        command.ItemName.ShouldBe("sword");
    }

    [Fact]
    public void QuitGame_is_created_from_empty_args()
    {
        var request = Requests.QuitGame.CreateRequest([]);

        request.ShouldBeOfType<Requests.QuitGame>();
    }

    [Fact]
    public void UseItemCommand_parses_item_and_target_from_args()
    {
        var request = Requests.UseItemCommand.CreateRequest(["key", "on", "door"]);

        var command = request.ShouldBeOfType<Requests.UseItemCommand>();
        command.ItemName.ShouldBe("key");
        command.TargetName.ShouldBe("door");
    }
}
