using Questline.Domain;
using Questline.Engine.Commands;

namespace Questline.Engine.Handlers;

public class GoCommandHandler : ICommandHandler<GoCommand>
{
    public CommandResult Execute(GameState state, GoCommand command)
    {
        var currentRoom = state.World.GetRoom(state.Player.Location);

        if (!currentRoom.Exits.TryGetValue(command.Direction, out var destinationId))
        {
            return new ErrorResult($"There is no exit to the {command.Direction}.");
        }

        state.Player.Location = destinationId;

        var newRoom = state.World.GetRoom(destinationId);
        var exits = newRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        return new MovedResult(newRoom.Name, newRoom.Description, exits);
    }
}
