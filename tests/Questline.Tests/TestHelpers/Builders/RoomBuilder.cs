using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Tests.TestHelpers.Builders.Templates;
using TestStack.Dossier;

namespace Questline.Tests.TestHelpers.Builders;

public class RoomBuilder : TestDataBuilder<Room, RoomBuilder>
{
    private readonly Dictionary<Direction, Exit> _exits    = new();
    private readonly List<Feature>               _features = [];
    private readonly List<Item>                  _items    = [];

    public RoomBuilder WithId(string id) =>
        Set(x => x.Id, id);

    public RoomBuilder WithName(string name) =>
        Set(x => x.Name, name);

    public RoomBuilder WithDescription(string description) =>
        Set(x => x.Description, description);

    public RoomBuilder WithExit(Direction direction, string destinationId) =>
        WithExit(direction, Exits.Default.WithDestination(destinationId));

    public RoomBuilder WithExit(Direction direction, Exit exit)
    {
        _exits[direction] = exit;
        return this;
    }

    public RoomBuilder WithItem(Item item)
    {
        _items.Add(item);
        return this;
    }

    public RoomBuilder WithFeature(Feature feature)
    {
        _features.Add(feature);
        return this;
    }

    protected override Room BuildObject()
    {
        return new Room
        {
            Id          = Get(x => x.Id),
            Name        = Get(x => x.Name),
            Description = Get(x => x.Description),
            Exits       = _exits,
            Items       = _items,
            Features    = _features
        };
    }
}
