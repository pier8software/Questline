using Questline.Framework.Domain;

namespace Questline.Framework.Persistence;

public interface IPersistenceMapper<TEntity, TDocument>
    where TEntity : DomainEntity
    where TDocument : Document
{
    TEntity   From(TDocument document);
    TDocument To(TEntity     entity);
}
