using Questline.Domain.Adventures.Entity;
using Questline.Engine.Repositories;
using Questline.Framework.Persistence;

namespace Questline.Engine.Persistence.Adventures;

public class AdventureRepository(
    IDataContext                                    dataContext,
    IPersistenceMapper<Adventure, AdventureDocument> mapper)
    : Repository<Adventure, AdventureDocument>(dataContext, mapper), IAdventureRepository
{
    public async Task<Adventure> GetById(string id) => await Load(id);
}
