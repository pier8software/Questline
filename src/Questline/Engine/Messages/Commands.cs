using Questline.Domain;
using Questline.Framework.Mediator;

namespace Questline.Engine.Messages;

public static class Commands
{
    public record DropCommand(string ItemName) : ICommand;

    public record GetCommand(string ItemName) : ICommand;

    public record GoCommand(Direction Direction) : ICommand;

    public record InventoryCommand : ICommand;

    public record LookCommand : ICommand;

    public record QuitCommand : ICommand;
}
