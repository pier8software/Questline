using Questline.Domain.Messages;
using Questline.Domain.Players.Messages;
using Questline.Domain.Shared.Data;
using Questline.Framework.Mediator;
using Commands = Questline.Domain.Players.Messages.Commands;

namespace Questline.Domain.Players.Handlers;

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
        return new Events.PlayerMoved(newRoom.Name, newRoom.Description, exits, items);
    }
}
