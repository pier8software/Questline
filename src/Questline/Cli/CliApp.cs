using Questline.Engine.Core;
using Questline.Engine.Messages;

namespace Questline.Cli;

public class CliApp(
    IConsole          console,
    ResponseFormatter formatter,
    GameEngine                    engine)
{
    public void Run()
    {
        var response = engine.ProcessInput(null);
        console.WriteLine(formatter.Format(response));

        while (engine.Phase != GamePhase.Ended)
        {
            console.Write("> ");
            var input = console.ReadLine();

            if (input is null)
            {
                break;
            }

            response = engine.ProcessInput(input);
            console.WriteLine(formatter.Format(response));

            if (response is Responses.GameQuitedResponse)
            {
                break;
            }
        }
    }
}
