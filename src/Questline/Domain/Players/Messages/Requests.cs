using Questline.Domain.Rooms.Entity;
using Questline.Engine;
using Questline.Engine.InputParsers;
using Questline.Framework.Mediator;

namespace Questline.Domain.Players.Messages;

public static class Requests
{
    [Verbs("go", "move", "walk")]
    public record MovePlayerCommand(Direction Direction) : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new MovePlayerCommand(Enum.Parse<Direction>(args[0]));
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
}
