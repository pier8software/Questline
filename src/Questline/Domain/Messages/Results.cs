using Questline.Framework.Mediator;

namespace Questline.Domain.Messages;

public static class Results
{
    public record CommandError(string Message) : CommandResult(Message, false);

    public record GameQuited() : CommandResult("Goodbye!");
}
