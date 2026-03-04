using Questline.Domain.Rooms.Entity;
using TestStack.Dossier;

namespace Questline.Tests.TestHelpers.Builders;

public class FeatureBuilder : TestDataBuilder<Feature, FeatureBuilder>
{
    public FeatureBuilder WithId(string id) =>
        Set(x => x.Id, id);

    public FeatureBuilder WithName(string name) =>
        Set(x => x.Name, name);

    public FeatureBuilder WithKeywords(List<string> keywords) =>
        Set(x => x.Keywords, keywords);

    public FeatureBuilder WithDescription(string description) =>
        Set(x => x.Description, description);

    protected override Feature BuildObject()
    {
        return new Feature
        {
            Id          = Get(x => x.Id),
            Name        = Get(x => x.Name),
            Keywords    = Get(x => x.Keywords),
            Description = Get(x => x.Description)
        };
    }
}
