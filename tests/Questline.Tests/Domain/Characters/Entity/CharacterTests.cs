using Questline.Domain.Characters.Entity;

namespace Questline.Tests.Domain.Characters.Entity;

public class CharacterTests
{
    [Fact]
    public void Location_is_mutable()
    {
        var character = new Character("TestHero", Race.Human, CharacterClass.Fighter) { Location = "start" };
        character.Location.ShouldBe("start");

        character.Location = "end";
        character.Location.ShouldBe("end");
    }

    [Fact]
    public void Inventory_starts_empty()
    {
        var character = new Character("TestHero", Race.Human, CharacterClass.Fighter) { Location = "start" };

        character.Inventory.IsEmpty.ShouldBeTrue();
    }
}
