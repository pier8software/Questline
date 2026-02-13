using Questline.Domain;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class GoCommandHandler : ICommandHandler<Commands.GoCommand>
{
    public CommandResult Execute(GameState state, Commands.GoCommand command)
    {
        var currentRoom = state.World.GetRoom(state.Player.Location);

        if (!currentRoom.Exits.TryGetValue(command.Direction, out var exit))
        {
            return new Results.ErrorResult($"There is no exit to the {command.Direction}.");
        }

        state.Player.Location = exit.Destination;

        var newRoom = state.World.GetRoom(exit.Destination);
        var exits = newRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = newRoom.Items.Items.Select(i => i.Name).ToList();
        return new Results.MovedResult(newRoom.Name, newRoom.Description, exits, items);
    }
}
