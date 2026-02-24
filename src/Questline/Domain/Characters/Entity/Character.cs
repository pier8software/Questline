using System.Collections.Immutable;
using Questline.Domain.Shared.Entity;

namespace Questline.Domain.Characters.Entity;

public record Character
{
    public string Name { get; private init; } = null!;
    public Race Race { get; private init; }
    public CharacterClass Class { get; private init; }
    public int Level { get; private init; }
    public int Experience { get; init; }
    public AbilityScores AbilityScores { get; private init; } = null!;
    public HitPoints HitPoints { get; private init; } = null!;
    public string Location { get; private init; } = null!;
    public ImmutableList<Item> Inventory { get; private init; } = ImmutableList<Item>.Empty;

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
            Location = location,
            Inventory = ImmutableList<Item>.Empty
        };
    }

    public Character MoveTo(string locationId) => this with { Location = locationId };

    public Character AddInventoryItem(Item item) => this with { Inventory = Inventory.Add(item) };

    public Character RemoveInventoryItem(Item item) => this with { Inventory = Inventory.Remove(item) };

    public Item? FindInventoryItemByName(string name) =>
        Inventory.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public string ToCharacterSummary()
    {
        return $"""
                Name: {Name}
                Race: {Race}
                Class: {Class}
                Level: {Level}
                HP: {HitPoints.CurrentHitPoints}/{HitPoints.MaxHitPoints}
                XP: {Experience}

                Ability Scores:
                  STR: {AbilityScores.Strength}
                  INT: {AbilityScores.Intelligence}
                  WIS: {AbilityScores.Wisdom}
                  DEX: {AbilityScores.Dexterity}
                  CON: {AbilityScores.Constitution}
                  CHA: {AbilityScores.Charisma}
                """;
    }
}
