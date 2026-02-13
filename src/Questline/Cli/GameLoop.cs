using Questline.Domain;
using Questline.Engine;
using Questline.Engine.InputParsers;

namespace Questline.Cli;

public class GameLoop(IConsole console, Parser parser, CommandDispatcher dispatcher, GameState state)
{
    public void Run()
    {
        DisplayCurrentRoom();

        while (true)
        {
            console.Write("> ");
            var input = console.ReadLine();

            if (input is null)
            {
                break;
            }

            var parseOutput = parser.Parse(input);
            var result = parseOutput.Match(
                command => dispatcher.Dispatch(state, command),
                error => new ErrorResult(error.Message));

            console.WriteLine(result.Message);

            if (result is QuitResult)
            {
                break;
            }
        }
    }

    private void DisplayCurrentRoom()
    {
        var room = state.World.GetRoom(state.Player.Location);
        var exits = room.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = room.Items.Items.Select(i => i.Name).ToList();
        var lookResult = new LookResult(room.Name, room.Description, exits, items);
        console.WriteLine(lookResult.Message);
    }
}
