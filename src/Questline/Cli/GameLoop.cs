using Questline.Domain.Shared;
using Questline.Engine.InputParsers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

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
                error => new Results.CommandError(error.Message));

            console.WriteLine(result.Message);

            if (result is Results.GameQuited)
            {
                break;
            }
        }
    }

    private void DisplayCurrentRoom()
    {
        var room = state.GetRoom(state.Player.Location);
        var exits = room.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = room.Items.Items.Select(i => i.Name).ToList();
        var lookResult = new Results.RoomViewed(room.Name, room.Description, exits, items);
        console.WriteLine(lookResult.Message);
    }
}
