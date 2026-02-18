using Questline.Domain.Rooms.Entity;
using Questline.Framework.Mediator;

namespace Questline.Engine.Messages;

public static class Requests
{
    [Verbs("use")]
    public record UseItemCommand(string ItemName, string? TargetName) : IRequest
    {
        public static IRequest CreateRequest(string[] args)
        {
            var input = string.Join(" ", args);
            var parts = input.Split(" on ", 2, StringSplitOptions.TrimEntries);
            var itemName = parts[0];
            var targetName = parts.Length > 1 ? parts[1] : null;
            return new UseItemCommand(itemName, targetName);
        }
    }

    [Verbs("examine", "inspect")]
    public record ExamineCommand(string TargetName) : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new ExamineCommand(string.Join(" ", args));
    }

    [Verbs("go", "move", "walk")]
    public record MovePlayerCommand(Direction Direction) : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new MovePlayerCommand(Enum.Parse<Direction>(args[0], true));
    }

    [Verbs("drop")]
    public record DropItemCommand(string ItemName) : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new DropItemCommand(string.Join(" ", args));
    }

    [Verbs("inventory", "inv", "i")]
    public record GetPlayerInventoryQuery : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new GetPlayerInventoryQuery();
    }

    [Verbs("take", "get")]
    public record TakeItemCommand(string ItemName) : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new TakeItemCommand(string.Join(" ", args));
    }

    [Verbs("look", "l")]
    public record GetRoomDetailsQuery : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new GetRoomDetailsQuery();
    }

    [Verbs("quit", "exit")]
    public record QuitGame : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new QuitGame();
    }
}
