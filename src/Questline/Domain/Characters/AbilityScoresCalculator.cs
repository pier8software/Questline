using Questline.Domain.Characters.Entity;
using Questline.Engine.Characters;

namespace Questline.Domain.Characters;

public static class AbilityScoresCalculator
{
    public static AbilityScores Calculate(IDice dice) => new(
        new AbilityScore(dice.Roll(1, 6).Sum()),
        new AbilityScore(dice.Roll(1, 6).Sum()),
        new AbilityScore(dice.Roll(1, 6).Sum()),
        new AbilityScore(dice.Roll(1, 6).Sum()),
        new AbilityScore(dice.Roll(1, 6).Sum()),
        new AbilityScore(dice.Roll(1, 6).Sum())
    );
}

public static class HitPointsCalculator
{
    public static HitPoints Calculate(CharacterClass? @class, IDice dice)
    {
        return @class switch
        {
            CharacterClass.Fighter => new HitPoints(8, dice.Roll(1, 8).Sum()),
            _ => throw new ArgumentException($"Unknown class: {@class}")
        };
    }
}
