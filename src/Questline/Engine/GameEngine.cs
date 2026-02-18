using Questline.Domain.Shared.Data;
using Questline.Engine.InputParsers;
using Questline.Framework.Mediator;

namespace Questline.Engine;

public class GameEngine(Parser parser, RequestSender dispatcher, GameState state)
{
    public IResponse ProcessInput(string input)
    {
        var parseResult = parser.Parse(input);
        if (!parseResult.IsSuccess)
        {
            return parseResult.Error!;
        }

        return dispatcher.Send(state, parseResult.Request!);
    }
}
