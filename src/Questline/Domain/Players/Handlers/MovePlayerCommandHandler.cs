using Questline.Domain.Shared.Data;
using Questline.Framework.Mediator;
using Requests = Questline.Domain.Players.Messages.Requests;
using Responses = Questline.Domain.Players.Messages.Responses;

namespace Questline.Domain.Players.Handlers;

public class MovePlayerCommandHandler : IRequestHandler<Requests.MovePlayerCommand>
{
    public IResponse Handle(GameState state, Requests.MovePlayerCommand command)
    {
        var currentRoom = state.GetRoom(state.Player.Location);

        if (!currentRoom.Exits.TryGetValue(command.Direction, out var exit))
        {
            return Responses.PlayerMovedResponse.Error($"There is no exit to the {command.Direction}.");
        }

        state.Player.Location = exit.Destination;

        var newRoom = state.GetRoom(exit.Destination);
        var exits = newRoom.Exits.Keys.Select(d => d.ToString()).ToList();
        var items = newRoom.Items.Items.Select(i => i.Name).ToList();

        return Responses.PlayerMovedResponse.Success(newRoom.Name, newRoom.Description, exits, items);
    }
}
