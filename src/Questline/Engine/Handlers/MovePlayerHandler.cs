using Questline.Domain;
using Questline.Domain.Shared;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class MovePlayerHandler : ICommandHandler<Commands.MovePlayer>
{
    public CommandResult Execute(GameState state, Commands.MovePlayer command)
    {
        var currentRoom = state.GetRoom(state.Player.Location);

        if (!currentRoom.Exits.TryGetValue(command.Direction, out var exit))
        {
            return new Results.CommandError($"There is no exit to the {command.Direction}.");
        }

        state.Player.Location = exit.Destination;

        var newRoom = state.GetRoom(exit.Destination);
        var exits = newRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = newRoom.Items.Items.Select(i => i.Name).ToList();
        return new Results.PlayerMoved(newRoom.Name, newRoom.Description, exits, items);
    }
}
