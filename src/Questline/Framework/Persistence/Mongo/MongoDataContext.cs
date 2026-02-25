using MongoDB.Driver;

namespace Questline.Framework.Persistence.Mongo;

public class MongoDataContext(IMongoDatabase database) : IDataContext
{
    public async Task<TEntity> Load<TEntity>(string id, CancellationToken cancellationToken) where TEntity : Document =>
        await database.Documents<TEntity>().Load(id, cancellationToken);

    public async Task StoreDocument<TEntity>(TEntity document, CancellationToken cancellationToken)
        where TEntity : Document =>
        await database.Documents<TEntity>()
            .StoreDocument(document, cancellationToken);
}
