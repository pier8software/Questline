using Questline.Framework.Domain;

namespace Questline.Framework.Persistence;

public abstract class Repository<TEntity, TDocument>(
    IDataContext                           dataContext,
    IPersistenceMapper<TEntity, TDocument> mapper)
    where TEntity : DomainEntity
    where TDocument : Document
{
    public virtual async Task<TEntity> Load(string entityId, CancellationToken cancellationToken = default)
    {
        var document = await dataContext.Load<TDocument>(entityId, cancellationToken);

        var entity = mapper.From(document);
        ((IDomainEntity)entity).Metadata.Add(
            nameof(Document.ConcurrencyTag),
            document.ConcurrencyTag);

        return entity;
    }

    public virtual async Task Save(TEntity entity, CancellationToken cancellationToken = default)
    {
        var document = mapper.To(entity);

        ((IDomainEntity)entity).Metadata.TryGetValue(
            nameof(Document.ConcurrencyTag),
            out var concurrencyTag);

        document.ConcurrencyTag = concurrencyTag as Guid?;

        await dataContext.StoreDocument(document, cancellationToken);
    }
}
