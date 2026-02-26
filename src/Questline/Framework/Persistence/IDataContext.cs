namespace Questline.Framework.Persistence;

public interface IDataContext
{
    Task<TEntity> Load<TEntity>(string id, CancellationToken cancellationToken) where TEntity : Document;

    Task StoreDocument<TEntity>(TEntity document, CancellationToken cancellationToken) where TEntity : Document;
}
