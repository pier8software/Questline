using Questline.Domain.Adventures.Entity;
using TestStack.Dossier;

namespace Questline.Tests.TestHelpers.Builders;

public class AdventureBuilder : TestDataBuilder<Adventure, AdventureBuilder>
{
    public AdventureBuilder WithId(string id) =>
        Set(x => x.Id, id);

    public AdventureBuilder WithName(string name) =>
        Set(x => x.Name, name);

    public AdventureBuilder WithDescription(string description) =>
        Set(x => x.Description, description);

    public AdventureBuilder WithStartingRoomId(string startingRoomId) =>
        Set(x => x.StartingRoomId, startingRoomId);

    protected override Adventure BuildObject()
    {
        return new Adventure
        {
            Id             = Get(x => x.Id),
            Name           = Get(x => x.Name),
            Description    = Get(x => x.Description),
            StartingRoomId = Get(x => x.StartingRoomId)
        };
    }
}
