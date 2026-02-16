using Questline.Domain.Messages;
using Questline.Engine;

namespace Questline.Cli;

public class CliApp(IConsole console, GameEngine engine)
{
    public void Run()
    {
        var initialRoom = engine.GetInitialRoom();
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

            if (result is Results.GameQuited)
            {
                break;
            }
        }
    }
}
