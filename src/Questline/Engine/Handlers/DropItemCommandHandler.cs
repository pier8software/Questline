using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Engine.Repositories;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class DropItemCommandHandler(
    IGameSession           session,
    IPlaythroughRepository playthroughRepository,
    IRoomRepository        roomRepository) : IRequestHandler<Requests.DropItemCommand>
{
    public async Task<IResponse> Handle(Actor actor, Requests.DropItemCommand command)
    {
        var playthrough = await playthroughRepository.GetById(session.PlaythroughId!);

        var actingCharacter = ActingCharacterResolver.Resolve(actor, playthrough.Party);
        var item            = actingCharacter.FindInventoryItemByName(command.ItemName);

        if (item is null)
        {
            return new ErrorResponse($"You are not carrying '{command.ItemName}'.");
        }

        actingCharacter.RemoveInventoryItem(item);

        var room      = await roomRepository.GetById(playthrough.Location);
        var roomItems = playthrough.GetRecordedRoomItems(room.Id) ?? room.Items.ToList();
        roomItems.Add(item);
        playthrough.RecordRoomItems(room.Id, roomItems);
        await playthroughRepository.Save(playthrough);

        var characterName = actor is CharacterActor ? actingCharacter.Name : null;
        return new Responses.ItemDroppedResponse(item.Name, characterName);
    }
}
