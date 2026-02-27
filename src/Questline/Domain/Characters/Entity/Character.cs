using Questline.Domain.Characters.Data;
using Questline.Framework.Domain;

namespace Questline.Domain.Characters.Entity;

public class Character : DomainEntity
{
    public string         Name          { get; private init; } = null!;
    public Race           Race          { get; private init; }
    public CharacterClass Class         { get; private init; }
    public int            Level         { get; private init; }
    public int            Experience    { get; init; }
    public AbilityScores  AbilityScores { get; private init; } = null!;
    public HitPoints      HitPoints     { get; private init; } = null!;

    public static Character Create(
        string          id,
        string          name,
        Race?           race,
        CharacterClass? characterClass,
        HitPoints       hitPoints,
        AbilityScores   abilityScores)
    {
        return new Character
        {
            Id            = id,
            Name          = name,
            Race          = race.GetValueOrDefault(),
            Class         = characterClass.GetValueOrDefault(),
            Level         = 1,
            Experience    = 0,
            AbilityScores = abilityScores,
            HitPoints     = hitPoints
        };
    }

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
