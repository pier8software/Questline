using Questline.Domain;
using Questline.Domain.Entities;
using Questline.Framework.Mediator;

namespace Questline.Engine.Messages;

public static class Commands
{
    public record DropItem(string ItemName) : ICommand;

    public record TakeItem(string ItemName) : ICommand;

    public record MovePlayer(Direction Direction) : ICommand;

    public record LoadInventory : ICommand;

    public record ViewRoom : ICommand;

    public record QuitGame : ICommand;
}
