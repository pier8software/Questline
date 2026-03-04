using Questline.Domain.Shared.Entity;
using TestStack.Dossier;

namespace Questline.Tests.TestHelpers.Builders;

public class ItemBuilder : TestDataBuilder<Item, ItemBuilder>
{
    public ItemBuilder WithId(string id) =>
        Set(x => x.Id, id);

    public ItemBuilder WithName(string name) =>
        Set(x => x.Name, name);

    public ItemBuilder WithDescription(string description) =>
        Set(x => x.Description, description);

    protected override Item BuildObject()
    {
        return new Item
        {
            Id          = Get(x => x.Id),
            Name        = Get(x => x.Name),
            Description = Get(x => x.Description)
        };
    }
}
