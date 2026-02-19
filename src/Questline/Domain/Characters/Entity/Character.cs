namespace Questline.Domain.Characters.Entity;

public record Character(
    string Name,
    Race Race,
    CharacterClass Class,
    int Level = 1,
    int Experience = 0,
    CharacterStats? Stats = null);
