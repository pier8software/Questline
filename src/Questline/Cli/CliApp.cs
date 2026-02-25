using Questline.Engine.Core;
using Questline.Engine.Messages;

namespace Questline.Cli;

public class CliApp(
    IConsole console,
    GameEngine engine,
    ResponseFormatter formatter)
{
    public void Run()
    {
        console.WriteLine("Welcome to Questline!");

        var response = engine.LoadWorld("the-goblins-lair");
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

            if (response is Responses.GameQuitResponse)
            {
                break;
            }
        }
    }
}
