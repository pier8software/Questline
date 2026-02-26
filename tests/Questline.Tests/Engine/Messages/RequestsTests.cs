using Questline.Domain.Rooms.Entity;
using Questline.Engine.Messages;

namespace Questline.Tests.Engine.Messages;

public class RequestsTests
{
    [Fact]
    public void Can_create_DropItemCommand()
    {
        var request = Requests.DropItemCommand.CreateRequest(["sword"]);

        var command = request.ShouldBeOfType<Requests.DropItemCommand>();
        command.ItemName.ShouldBe("sword");
    }

    [Fact]
    public void Can_create_ExamineCommand()
    {
        var request = Requests.ExamineCommand.CreateRequest(["sword"]);

        var command = request.ShouldBeOfType<Requests.ExamineCommand>();
        command.TargetName.ShouldBe("sword");
    }

    [Fact]
    public void Can_create_GetRoomDetailsQuery()
    {
        var request = Requests.GetRoomDetailsQuery.CreateRequest([]);

        request.ShouldBeOfType<Requests.GetRoomDetailsQuery>();
    }

    [Fact]
    public void Can_create_GetPlayerInventoryQuery()
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
    public void Can_create_MovePlayerCommand(string direction, Direction expectedDirection)
    {
        var request = Requests.MovePlayerCommand.CreateRequest([direction]);

        var command = request.ShouldBeOfType<Requests.MovePlayerCommand>();
        command.Direction.ShouldBe(expectedDirection);
    }

    [Fact]
    public void Can_create_TakeItemCommand()
    {
        var request = Requests.TakeItemCommand.CreateRequest(["sword"]);

        var command = request.ShouldBeOfType<Requests.TakeItemCommand>();
        command.ItemName.ShouldBe("sword");
    }

    [Fact]
    public void Can_create_QuitGame()
    {
        var request = Requests.QuitGame.CreateRequest([]);

        request.ShouldBeOfType<Requests.QuitGame>();
    }

    [Fact]
    public void Can_create_UseItemCommand()
    {
        var request = Requests.UseItemCommand.CreateRequest(["key", "on", "door"]);

        var command = request.ShouldBeOfType<Requests.UseItemCommand>();
        command.ItemName.ShouldBe("key");
        command.TargetName.ShouldBe("door");
    }
}
