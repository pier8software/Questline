namespace Questline.Domain.Characters.Entity;

public record CharacterStats(
    int MaxHealth,
    int CurrentHealth,
    int Strength,
    int Intelligence,
    int Wisdom,
    int Dexterity,
    int Constitution,
    int Charisma);
