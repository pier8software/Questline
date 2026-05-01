using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class GetPlayerInventoryQueryHandler(
    IGameSession           session,
    IPlaythroughRepository playthroughRepository) : IRequestHandler<Requests.GetPlayerInventoryQuery>
{
    public async Task<IResponse> Handle(Actor actor, Requests.GetPlayerInventoryQuery command)
    {
        var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);

        if (actor is CharacterActor characterActor)
        {
            var pc = characterActor.Character;
            return new Responses.InventoryResponse(
                [(pc.Name, pc.Inventory.Select(i => i.Name).ToList())]);
        }

        var partyInventory = playthrough.Party.Members
            .Select(c => (c.Name, (IReadOnlyList<string>)c.Inventory.Select(i => i.Name).ToList()))
            .ToList();

        return new Responses.InventoryResponse(partyInventory);
    }
}
