using MongoDB.Driver;
using Questline.Domain.Playthroughs.Data;
using Questline.Domain.Playthroughs.Entity;
using Questline.Engine.Repositories;
using Questline.Framework.Persistence;
using Questline.Framework.Persistence.Mongo;

namespace Questline.Engine.Persistence.Playthroughs;

public class PlaythroughRepository(
    IDataContext                                        dataContext,
    IPersistenceMapper<Playthrough, PlaythroughDocument> mapper,
    IMongoDatabase                                      database)
    : Repository<Playthrough, PlaythroughDocument>(dataContext, mapper), IPlaythroughRepository
{
    public async Task<Playthrough> GetById(string id) => await Load(id);

    public async Task Save(Playthrough playthrough) => await base.Save(playthrough);

    public async Task<IReadOnlyList<PlaythroughSummary>> FindByUsername(string username)
    {
        var collection = database.Documents<PlaythroughDocument>();
        var filter = Builders<PlaythroughDocument>.Filter.Eq(d => d.Username, username);
        var documents = await collection.Find(filter).ToListAsync();

        return documents
            .Select(d => new PlaythroughSummary(d.Id, d.CharacterName, d.AdventureId, d.Location))
            .ToList();
    }
}
