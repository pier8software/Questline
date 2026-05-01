using Questline.Domain.Characters.Entity;
using Questline.Engine.Characters;

namespace Questline.Domain.Characters;

public static class AbilityScoresCalculator
{
    public static AbilityScores Calculate(IDice dice) => new(
        new AbilityScore(dice.Roll(3, 6)),
        new AbilityScore(dice.Roll(3, 6)),
        new AbilityScore(dice.Roll(3, 6)),
        new AbilityScore(dice.Roll(3, 6)),
        new AbilityScore(dice.Roll(3, 6)),
        new AbilityScore(dice.Roll(3, 6))
    );
}

public static class HitPointsCalculator
{
    public static HitPoints Calculate(CharacterClass? @class, IDice dice)
    {
        return @class switch
        {
            CharacterClass.Fighter => new HitPoints(max: 8, current: dice.Roll(1, 8)),
            _                      => throw new ArgumentException($"Unknown class: {@class}")
        };
    }
}
