using Questline.Domain;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class LookCommandHandler : ICommandHandler<Commands.LookCommand>
{
    public CommandResult Execute(GameState state, Commands.LookCommand command)
    {
        var room = state.World.GetRoom(state.Player.Location);
        var exits = room.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = room.Items.Items.Select(i => i.Name).ToList();
        return new Results.LookResult(room.Name, room.Description, exits, items);
    }
}
