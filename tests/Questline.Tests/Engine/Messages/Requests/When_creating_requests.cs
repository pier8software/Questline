using Questline.Domain.Rooms.Entity;

namespace Questline.Tests.Engine.Messages.Requests;

public class When_creating_requests
{
    [Fact]
    public void DropItemCommand_is_created_from_args()
    {
        var request = Questline.Engine.Messages.Requests.DropItemCommand.CreateRequest(["sword"]);

        var command = request.ShouldBeOfType<Questline.Engine.Messages.Requests.DropItemCommand>();
        command.ItemName.ShouldBe("sword");
    }

    [Fact]
    public void ExamineCommand_is_created_from_args()
    {
        var request = Questline.Engine.Messages.Requests.ExamineCommand.CreateRequest(["sword"]);

        var command = request.ShouldBeOfType<Questline.Engine.Messages.Requests.ExamineCommand>();
        command.TargetName.ShouldBe("sword");
    }

    [Fact]
    public void GetRoomDetailsQuery_is_created_from_empty_args()
    {
        var request = Questline.Engine.Messages.Requests.GetRoomDetailsQuery.CreateRequest([]);

        request.ShouldBeOfType<Questline.Engine.Messages.Requests.GetRoomDetailsQuery>();
    }

    [Fact]
    public void GetPlayerInventoryQuery_is_created_from_empty_args()
    {
        var request = Questline.Engine.Messages.Requests.GetPlayerInventoryQuery.CreateRequest([]);

        request.ShouldBeOfType<Questline.Engine.Messages.Requests.GetPlayerInventoryQuery>();
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
        var request = Questline.Engine.Messages.Requests.MovePlayerCommand.CreateRequest([direction]);

        var command = request.ShouldBeOfType<Questline.Engine.Messages.Requests.MovePlayerCommand>();
        command.Direction.ShouldBe(expectedDirection);
    }

    [Fact]
    public void TakeItemCommand_is_created_from_args()
    {
        var request = Questline.Engine.Messages.Requests.TakeItemCommand.CreateRequest(["sword"]);

        var command = request.ShouldBeOfType<Questline.Engine.Messages.Requests.TakeItemCommand>();
        command.ItemName.ShouldBe("sword");
    }

    [Fact]
    public void QuitGame_is_created_from_empty_args()
    {
        var request = Questline.Engine.Messages.Requests.QuitGame.CreateRequest([]);

        request.ShouldBeOfType<Questline.Engine.Messages.Requests.QuitGame>();
    }

    [Fact]
    public void UseItemCommand_parses_item_and_target_from_args()
    {
        var request = Questline.Engine.Messages.Requests.UseItemCommand.CreateRequest(["key", "on", "door"]);

        var command = request.ShouldBeOfType<Questline.Engine.Messages.Requests.UseItemCommand>();
        command.ItemName.ShouldBe("key");
        command.TargetName.ShouldBe("door");
    }
}
