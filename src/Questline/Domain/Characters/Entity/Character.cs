using Questline.Domain.Shared.Entity;

namespace Questline.Domain.Characters.Entity;

public class Character(
    string name,
    Race race,
    CharacterClass @class,
    int level = 1,
    int experience = 0,
    CharacterStats? stats = null)
{
    public string Name { get; } = name;
    public Race Race { get; } = race;
    public CharacterClass Class { get; } = @class;
    public int Level { get; } = level;
    public int Experience { get; } = experience;
    public CharacterStats? Stats { get; } = stats;
    public string Location { get; set; } = "";
    public Inventory Inventory { get; init; } = new();
}
