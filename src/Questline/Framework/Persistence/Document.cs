namespace Questline.Framework.Persistence;

public abstract class Document<TId>
{
    public TId            Id             { get; set; } = default!;
    public Guid?          ConcurrencyTag { get; set; }
    public DateTimeOffset UpdatedAt      { get; set; }
}

public abstract class Document : Document<string>
{ }
