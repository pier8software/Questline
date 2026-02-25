namespace Questline.Framework.Domain;

public abstract class DomainEntity : IDomainEntity
{
    public string Id { get; } = null!;
    Dictionary<string, object?> IDomainEntity.Metadata { get; } = new();
}

public interface IDomainEntity
{
    string Id { get; }
    Dictionary<string, object?> Metadata { get; }
}
