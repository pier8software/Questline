using MongoDB.Driver;
using Questline.Domain.Playthroughs.Data;
using Questline.Domain.Playthroughs.Entity;
using Questline.Engine.Persistence.Adventures;
using Questline.Engine.Repositories;
using Questline.Framework.Persistence;
using Questline.Framework.Persistence.Mongo;

namespace Questline.Engine.Persistence.Playthroughs;

public class PlaythroughRepository(
    IDataContext                                         dataContext,
    IPersistenceMapper<Playthrough, PlaythroughDocument> mapper,
    IMongoDatabase                                       database)
    : Repository<Playthrough, PlaythroughDocument>(dataContext, mapper), IPlaythroughRepository
{
    public async Task<Playthrough> GetById(string id) => await Load(id);

    public async Task Save(Playthrough playthrough) => await base.Save(playthrough);

    public async Task<IReadOnlyList<PlaythroughSummary>> FindByUsername(string username)
    {
        var playthroughCollection = database.Documents<PlaythroughDocument>();
        var adventureCollection   = database.Documents<AdventureDocument>();

        var filter    = Builders<PlaythroughDocument>.Filter.Eq(d => d.Username, username);
        var documents = await playthroughCollection.Find(filter).ToListAsync();

        var adventureIds   = documents.Select(d => d.AdventureId).Distinct().ToList();
        var adventureFilter = Builders<AdventureDocument>.Filter.In(a => a.Id, adventureIds);
        var adventures     = await adventureCollection.Find(adventureFilter).ToListAsync();
        var nameById       = adventures.ToDictionary(a => a.Id, a => a.Name);

        return documents
            .Select(d => new PlaythroughSummary(
                d.Id,
                d.Party?.Members.FirstOrDefault()?.Name ?? "",
                nameById.GetValueOrDefault(d.AdventureId, d.AdventureId),
                d.Location))
            .ToList();
    }
}
