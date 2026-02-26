namespace Questline.Domain.Characters.Data;

public record CharacterSummary(
    string Name,
    string Race,
    string Class,
    int Level,
    int Experience,
    int MaxHitPoints,
    int CurrentHitPoints,
    AbilityScoresSummary AbilityScores);

public record AbilityScoresSummary(
    int Strength,
    int Intelligence,
    int Wisdom,
    int Dexterity,
    int Constitution,
    int Charisma);
