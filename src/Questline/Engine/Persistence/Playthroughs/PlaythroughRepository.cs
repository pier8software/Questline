using Questline.Domain.Playthroughs.Entity;
using Questline.Engine.Repositories;
using Questline.Framework.Persistence;

namespace Questline.Engine.Persistence.Playthroughs;

public class PlaythroughRepository(
    IDataContext                                        dataContext,
    IPersistenceMapper<Playthrough, PlaythroughDocument> mapper)
    : Repository<Playthrough, PlaythroughDocument>(dataContext, mapper), IPlaythroughRepository
{
    public async Task<Playthrough> GetById(string id) => await Load(id);

    public async Task Save(Playthrough playthrough) => await base.Save(playthrough);
}
