using Questline.Framework.Mediator;

namespace Questline.Domain.Rooms.Messages;

public static class Commands
{
    public record TakeRoomItem(string ItemName) : ICommand;

    public record ViewRoom : ICommand;
}
