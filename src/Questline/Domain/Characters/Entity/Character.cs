using Questline.Domain.Characters.Data;
using Questline.Domain.Shared.Entity;
using Questline.Framework.Domain;

namespace Questline.Domain.Characters.Entity;

public class Character : DomainEntity
{
    private readonly List<Item> _inventory = [];

    public string          Name          { get; private init; } = null!;
    public Race            Race          { get; private init; }
    public CharacterClass? Class         { get; private init; }
    public int             Level         { get; private init; }
    public int             Experience    { get; init; }
    public string          Occupation    { get; private init; } = "";
    public AbilityScores   AbilityScores { get; private init; } = null!;
    public HitPoints       HitPoints     { get; private init; } = null!;

    public IReadOnlyList<Item> Inventory
    {
        get => _inventory;
        init => _inventory = [..value];
    }

    public static Character Create(
        string          id,
        string          name,
        Race            race,
        CharacterClass? characterClass,
        HitPoints       hitPoints,
        AbilityScores   abilityScores,
        string          occupation = "",
        int             level = 0)
    {
        return new Character
        {
            Id            = id,
            Name          = name,
            Race          = race,
            Class         = characterClass,
            Level         = level,
            Experience    = 0,
            Occupation    = occupation,
            AbilityScores = abilityScores,
            HitPoints     = hitPoints
        };
    }

    public void AddInventoryItem(Item item) => _inventory.Add(item);

    public void RemoveInventoryItem(Item item) => _inventory.Remove(item);

    public Item? FindInventoryItemByName(string name) =>
        _inventory.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public CharacterSummary ToSummary() =>
        new(
            Name,
            Race.ToString(),
            Class?.ToString() ?? "Level 0",
            Occupation,
            Level,
            Experience,
            HitPoints.Max,
            HitPoints.Current,
            new AbilityScoresSummary(
                AbilityScores.Strength.Score,
                AbilityScores.Intelligence.Score,
                AbilityScores.Wisdom.Score,
                AbilityScores.Dexterity.Score,
                AbilityScores.Constitution.Score,
                AbilityScores.Charisma.Score));
}
