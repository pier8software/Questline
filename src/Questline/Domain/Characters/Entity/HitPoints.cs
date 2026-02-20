namespace Questline.Domain.Characters.Entity;

public record HitPoints(
    int MaxHitPoints,
    int CurrentHitPoints
);

public record AbilityScores(
    AbilityScore Strength,
    AbilityScore Intelligence,
    AbilityScore Wisdom,
    AbilityScore Dexterity,
    AbilityScore Constitution,
    AbilityScore Charisma);

public record AbilityScore(int Score);
