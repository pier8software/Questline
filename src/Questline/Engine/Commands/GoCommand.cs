using Questline.Domain;

namespace Questline.Engine.Commands;

public record GoCommand(Direction Direction) : ICommand;
