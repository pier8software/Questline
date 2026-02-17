using Questline.Engine;
using Questline.Engine.InputParsers;
using Questline.Framework.Mediator;

namespace Questline.Domain.Rooms.Messages;

public static class Requests
{
    [Verbs("take", "get")]
    public record TakeRoomItemCommand(string ItemName) : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new TakeRoomItemCommand(string.Join(" ", args));
    }

    [Verbs("look", "l")]
    public record GetRoomDetailsQuery : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new GetRoomDetailsQuery();
    }
}
