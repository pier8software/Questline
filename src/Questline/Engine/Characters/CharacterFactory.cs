using Questline.Domain.Characters;
using Questline.Domain.Characters.Entity;

namespace Questline.Engine.Characters;

public class CharacterFactory(IDice dice)
{
    public Character Create(string name)
    {
        if (!CharacterNameValidator.Validate(name))
        {
            throw new ArgumentException("Please give your character a name");
        }

        var strength = dice.Roll(3, 6).Sum();
        var intelligence = dice.Roll(3, 6).Sum();
        var wisdom = dice.Roll(3, 6).Sum();
        var dexterity = dice.Roll(3, 6).Sum();
        var constitution = dice.Roll(3, 6).Sum();
        var charisma = dice.Roll(3, 6).Sum();
        var currentHealth = dice.Roll(1, 8).Sum();

        var stats = new CharacterStats(
            8,
            currentHealth,
            strength,
            intelligence,
            wisdom,
            dexterity,
            constitution,
            charisma);

        return new Character(name, Race.Human, CharacterClass.Fighter, Stats: stats);
    }
}
