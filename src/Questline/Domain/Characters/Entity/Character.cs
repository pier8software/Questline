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
    public string Location { get; private set; } = null!;
    public Inventory Inventory { get; private init; } = new();

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
            Inventory = new()
        };
    }

    public void SetLocation(string locationId)
    {
        if (Location != locationId)
        {
            Location = locationId;
        }
    }

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
