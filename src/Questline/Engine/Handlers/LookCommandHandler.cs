using Questline.Domain;
using Questline.Engine.Commands;

namespace Questline.Engine.Handlers;

public class LookCommandHandler : ICommandHandler<LookCommand>
{
    public CommandResult Execute(GameState state, LookCommand command)
    {
        var room = state.World.GetRoom(state.Player.Location);
        var exits = room.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = room.Items.Items.Select(i => i.Name).ToList();
        return new LookResult(room.Name, room.Description, exits, items);
    }
}
