using Questline.Domain.Playthroughs.Entity;
using Questline.Framework.Persistence;

namespace Questline.Domain.Playthroughs.Persistence;

public class PlaythroughRepository(
    IDataContext                                         dataContext,
    IPersistenceMapper<Playthrough, PlaythroughDocument> mapper)
    : Repository<Playthrough, PlaythroughDocument>(dataContext, mapper)
{
}
