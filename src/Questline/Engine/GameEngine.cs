using Questline.Domain.Messages;
using Questline.Domain.Rooms.Messages;
using Questline.Domain.Shared.Data;
using Questline.Engine.InputParsers;
using Questline.Framework.Mediator;

namespace Questline.Engine;

public class GameEngine(Parser parser, CommandDispatcher dispatcher, GameState state)
{
    public CommandResult ProcessInput(string input)
    {
        var parseOutput = parser.Parse(input);
        return  parseOutput.Match(
            command => dispatcher.Dispatch(state, command),
            error => new Results.CommandError(error.Message));
    }

    public CommandResult GetInitialRoom()
    {
        var room = state.GetRoom(state.Player.Location);
        var exits = room.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = room.Items.Items.Select(i => i.Name).ToList();
        return new Events.RoomViewed(room.Name, room.Description, exits, items);
    }
}
