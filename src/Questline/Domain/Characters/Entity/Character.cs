using Questline.Domain.Characters.Data;
using Questline.Domain.Shared.Entity;

namespace Questline.Domain.Characters.Entity;

public class Character
{
    private readonly List<Item> _inventory = [];

    public string Name { get; private init; } = null!;
    public Race Race { get; private init; }
    public CharacterClass Class { get; private init; }
    public int Level { get; private init; }
    public int Experience { get; init; }
    public AbilityScores AbilityScores { get; private init; } = null!;
    public HitPoints HitPoints { get; private init; } = null!;
    public string Location { get; private set; } = null!;

    public IReadOnlyList<Item> Inventory
    {
        get => _inventory;
        private init => _inventory = [..value];
    }

    public static Character Create(
        string name,
        Race? race,
        CharacterClass? characterClass,
        HitPoints hitPoints,
        AbilityScores abilityScores,
        string location = "")
    {
        return new Character
        {
            Name = name,
            Race = race.GetValueOrDefault(),
            Class = characterClass.GetValueOrDefault(),
            Level = 1,
            Experience = 0,
            AbilityScores = abilityScores,
            HitPoints = hitPoints,
            Location = location
        };
    }

    public void MoveTo(string locationId) => Location = locationId;

    public void AddInventoryItem(Item item) => _inventory.Add(item);

    public void RemoveInventoryItem(Item item) => _inventory.Remove(item);

    public Item? FindInventoryItemByName(string name) =>
        _inventory.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public CharacterSummary ToSummary() =>
        new(
            Name,
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
