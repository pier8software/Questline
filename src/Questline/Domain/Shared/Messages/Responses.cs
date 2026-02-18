using Questline.Framework.Mediator;

namespace Questline.Domain.Shared.Messages;

public static class Responses
{
    public record GameQuited : IResponse
    {
        public string Message => "Goodbye!";
    }
}
