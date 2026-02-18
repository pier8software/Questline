using Questline.Engine;
using Questline.Engine.InputParsers;
using Questline.Framework.Mediator;

namespace Questline.Domain.Shared.Messages;

public static class Requests
{
    [Verbs("quit", "exit")]
    public record QuitGame : IRequest
    {
        public static IRequest CreateRequest(string[] args) => new QuitGame();
    }
}
