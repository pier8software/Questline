using Questline.Domain.Characters.Data;
using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Domain.Playthroughs.Data;
using Questline.Domain.Shared.Entity;
using Questline.Framework.Domain;

namespace Questline.Domain.Playthroughs.Entity;

public class Playthrough : DomainEntity
{
    private readonly HashSet<string>                _unlockedBarriers = [];
    private readonly Dictionary<string, List<Item>> _roomItems        = new();

    public required string Username       { get; init; }
    public required string AdventureId    { get; init; }
    public required string StartingRoomId { get; init; }
    public required Party  Party          { get; init; }
    public required string Location       { get; set; }
    public          int    Turns          { get; private set; }

    public IReadOnlyCollection<string> UnlockedBarriers
    {
        get => _unlockedBarriers;
        init => _unlockedBarriers = [..value];
    }

    public IReadOnlyDictionary<string, List<Item>> RoomItems
    {
        get => _roomItems;
        init => _roomItems = new Dictionary<string, List<Item>>(value);
    }

    /// <summary>Leader-shorthand for the current single-character APIs.
    /// Removed when handlers are routed by actor in a later task.</summary>
    public IReadOnlyList<Item> Inventory => Party.Members[0].Inventory;

    public static Playthrough Create(
        string username,
        string adventureId,
        string startingRoomId,
        Party  party)
    {
        return new Playthrough
        {
            Id             = Guid.NewGuid().ToString(),
            Username       = username,
            AdventureId    = adventureId,
            StartingRoomId = startingRoomId,
            Party          = party,
            Location       = startingRoomId
        };
    }

    public void MoveTo(string locationId) => Location = locationId;

    public void IncrementTurns() => Turns++;

    internal void RestoreTurns(int turns) => Turns = turns;

    /// <summary>Leader-shorthand — delegates to first party member.
    /// Removed when per-actor inventory routing lands in D10.</summary>
    public void AddInventoryItem(Item item) => Party.Members[0].AddInventoryItem(item);

    /// <summary>Leader-shorthand — delegates to first party member.
    /// Removed when per-actor inventory routing lands in D10.</summary>
    public void RemoveInventoryItem(Item item) => Party.Members[0].RemoveInventoryItem(item);

    /// <summary>Leader-shorthand — delegates to first party member.
    /// Removed when per-actor inventory routing lands in D10.</summary>
    public Item? FindInventoryItemByName(string name) =>
        Party.Members[0].FindInventoryItemByName(name);

    public bool IsBarrierUnlocked(string barrierId) => _unlockedBarriers.Contains(barrierId);

    public void UnlockBarrier(string barrierId) => _unlockedBarriers.Add(barrierId);

    public List<Item>? GetRecordedRoomItems(string roomId) =>
        _roomItems.TryGetValue(roomId, out var items) ? items : null;

    public void RecordRoomItems(string roomId, List<Item> items) =>
        _roomItems[roomId] = items;

    public PartySummary ToPartySummary() =>
        new(Party.Members.Select(c => c.ToSummary()).ToList(), Turns);
}
