using Questline.Framework.Mediator;

namespace Questline.Domain.Messages;

public static class Commands
{
    public record QuitGame : ICommand;
}
