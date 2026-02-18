using Questline.Engine;
using Questline.Engine.Messages;

namespace Questline.Cli;

public class CliApp(IConsole console, GameEngine engine)
{
    public void Run()
    {
        var initialRoom = engine.ProcessInput("look");
        console.WriteLine(initialRoom.Message);

        while (true)
        {
            console.Write("> ");
            var input = console.ReadLine();

            if (input is null)
            {
                break;
            }

            var result = engine.ProcessInput(input);

            console.WriteLine(result.Message);

            if (result is Responses.GameQuited)
            {
                break;
            }
        }
    }
}
