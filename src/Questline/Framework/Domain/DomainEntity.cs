namespace Questline.Framework.Domain;

public abstract class DomainEntity : IDomainEntity
{
    public string                             Id       { get; set; } = null!;
    Dictionary<string, object?> IDomainEntity.Metadata { get; set; } = new();
}

public interface IDomainEntity
{
    string                      Id       { get; set; }
    Dictionary<string, object?> Metadata { get; set; }
}
