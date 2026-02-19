using Questline.Domain.Rooms.Entity;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;

namespace Questline.Engine.Messages;

public static class Requests
{
    [Verbs("drop")]
    public record DropItemCommand(string ItemName) : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new DropItemCommand(string.Join(" ", args));
    }

    [Verbs("examine", "inspect")]
    public record ExamineCommand(string TargetName) : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new ExamineCommand(string.Join(" ", args));
    }

    [Verbs("look", "l")]
    public record GetRoomDetailsQuery : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new GetRoomDetailsQuery();
    }

    [Verbs("inventory", "inv", "i")]
    public record GetPlayerInventoryQuery : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new GetPlayerInventoryQuery();
    }

    [Verbs("go", "move", "walk")]
    public record MovePlayerCommand(Direction Direction) : IRequest
    {
        public static IRequest CreateRequest(string[] args) => DirectionParser.TryParse(args[0], out var direction)
            ? new MovePlayerCommand(direction)
            : new MovePlayerCommand(Direction.Invalid);
    }

    [Verbs("take", "get")]
    public record TakeItemCommand(string ItemName) : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new TakeItemCommand(string.Join(" ", args));
    }

    [Verbs("quit", "exit")]
    public record QuitGame : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new QuitGame();
    }

    [Verbs("version", "ver")]
    public record VersionQuery : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new VersionQuery();
    }

    [Verbs("stats")]
    public record StatsQuery : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new StatsQuery();
    }

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
}
