using Questline.Domain.Playthroughs.Entity;
using Questline.Domain.Playthroughs.Persistence;
using Questline.Engine.Content.Data;

namespace Questline.Engine.Services;

public class PlaythroughService(PlaythroughRepository repository)
{
    public async Task CreatePlaythrough(string playerId, AdventureContent adventureContent)
    {
        var playthrough = Playthrough.Create(playerId, adventureContent);
        await repository.Save(playthrough);
    }
}
