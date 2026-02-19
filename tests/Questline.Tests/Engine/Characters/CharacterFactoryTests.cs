using Questline.Domain.Characters.Entity;
using Questline.Engine.Characters;
using Questline.Tests.TestHelpers;

namespace Questline.Tests.Engine.Characters;

public class CharacterFactoryTests
{
    [Fact]
    public void Creates_character_with_given_name()
    {
        // 3d6 x 6 ability scores + 1d8 health = 19 rolls
        var dice = new FakeDice(3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4);
        var factory = new CharacterFactory(dice);

        var character = factory.Create("Thorin");

        character.Name.ShouldBe("Thorin");
    }

    [Fact]
    public void Creates_character_as_human_fighter()
    {
        var dice = new FakeDice(3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4);
        var factory = new CharacterFactory(dice);

        var character = factory.Create("Thorin");

        character.Race.ShouldBe(Race.Human);
        character.Class.ShouldBe(CharacterClass.Fighter);
    }

    [Fact]
    public void Creates_character_at_level_1()
    {
        var dice = new FakeDice(3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4);
        var factory = new CharacterFactory(dice);

        var character = factory.Create("Thorin");

        character.Level.ShouldBe(1);
        character.Experience.ShouldBe(0);
    }

    [Fact]
    public void Rolls_3d6_for_each_ability_score()
    {
        // STR: 4+5+6=15, INT: 3+3+3=9, WIS: 2+4+6=12, DEX: 1+1+1=3, CON: 6+6+6=18, CHA: 5+5+5=15
        var dice = new FakeDice(4, 5, 6, 3, 3, 3, 2, 4, 6, 1, 1, 1, 6, 6, 6, 5, 5, 5, 7);
        var factory = new CharacterFactory(dice);

        var character = factory.Create("Thorin");

        character.Stats!.Strength.ShouldBe(15);
        character.Stats.Intelligence.ShouldBe(9);
        character.Stats.Wisdom.ShouldBe(12);
        character.Stats.Dexterity.ShouldBe(3);
        character.Stats.Constitution.ShouldBe(18);
        character.Stats.Charisma.ShouldBe(15);
    }

    [Fact]
    public void Sets_max_health_to_8()
    {
        var dice = new FakeDice(3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 5);
        var factory = new CharacterFactory(dice);

        var character = factory.Create("Thorin");

        character.Stats!.MaxHealth.ShouldBe(8);
    }

    [Fact]
    public void Rolls_1d8_for_current_health()
    {
        var dice = new FakeDice(3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 5);
        var factory = new CharacterFactory(dice);

        var character = factory.Create("Thorin");

        character.Stats!.CurrentHealth.ShouldBe(5);
    }

    [Fact]
    public void Throws_when_name_is_invalid()
    {
        var dice = new FakeDice();
        var factory = new CharacterFactory(dice);

        Should.Throw<ArgumentException>(() => factory.Create(""));
    }
}
