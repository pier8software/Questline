using Questline.Domain.Characters.Data;
using Questline.Domain.Characters.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Framework.Domain;

namespace Questline.Domain.Playthroughs.Entity;

public class Playthrough : DomainEntity
{
    private readonly List<Item>                      _inventory        = [];
    private readonly HashSet<string>                 _unlockedBarriers = [];
    private readonly Dictionary<string, List<Item>>  _roomItems        = new();

    public required string         AdventureId    { get; init; }
    public required string         StartingRoomId { get; init; }
    public required string         CharacterName  { get; init; }
    public required Race           Race           { get; init; }
    public required CharacterClass Class          { get; init; }
    public required AbilityScores  AbilityScores  { get; init; }
    public required HitPoints      HitPoints      { get; init; }
    public          int            Level          { get; init; } = 1;
    public          int            Experience     { get; init; }
    public required string         Location       { get; set; }

    public IReadOnlyList<Item> Inventory
    {
        get => _inventory;
        init => _inventory = [..value];
    }

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

    public static Playthrough Create(
        string    adventureId,
        string    startingRoomId,
        Character character)
    {
        return new Playthrough
        {
            Id             = Guid.NewGuid().ToString(),
            AdventureId    = adventureId,
            StartingRoomId = startingRoomId,
            CharacterName  = character.Name,
            Race           = character.Race,
            Class          = character.Class,
            Level          = character.Level,
            Experience     = character.Experience,
            AbilityScores  = character.AbilityScores,
            HitPoints      = character.HitPoints,
            Location       = startingRoomId
        };
    }

    public void MoveTo(string locationId) => Location = locationId;

    public void AddInventoryItem(Item item) => _inventory.Add(item);

    public void RemoveInventoryItem(Item item) => _inventory.Remove(item);

    public Item? FindInventoryItemByName(string name) =>
        _inventory.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public bool IsBarrierUnlocked(string barrierId) => _unlockedBarriers.Contains(barrierId);

    public void UnlockBarrier(string barrierId) => _unlockedBarriers.Add(barrierId);

    public List<Item>? GetRecordedRoomItems(string roomId) =>
        _roomItems.TryGetValue(roomId, out var items) ? items : null;

    public void RecordRoomItems(string roomId, List<Item> items) =>
        _roomItems[roomId] = items;

    public CharacterSummary ToCharacterSummary() =>
        new(
            CharacterName,
            Race.ToString(),
            Class.ToString(),
            Level,
            Experience,
            HitPoints.MaxHitPoints,
            HitPoints.CurrentHitPoints,
            new AbilityScoresSummary(
                AbilityScores.Strength.Score,
                AbilityScores.Intelligence.Score,
                AbilityScores.Wisdom.Score,
                AbilityScores.Dexterity.Score,
                AbilityScores.Constitution.Score,
                AbilityScores.Charisma.Score));
}
