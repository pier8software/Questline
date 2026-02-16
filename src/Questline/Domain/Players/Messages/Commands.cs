using Questline.Domain.Rooms.Entity;
using Questline.Framework.Mediator;

namespace Questline.Domain.Players.Messages;

public static class Commands
{
    public record MovePlayer(Direction Direction) : ICommand;

    public record DropPlayerItem(string ItemName) : ICommand;

    public record LoadPlayerInventory : ICommand;
}
