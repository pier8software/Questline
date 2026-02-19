using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;

namespace Questline.Tests.Domain.Players.Entity;

public class PlayerTests
{
    [Fact]
    public void Has_character_property()
    {
        var character = new Character("TestHero", Race.Human, CharacterClass.Fighter) { Location = "start" };
        var player = new Player { Id = "player1", Character = character };

        player.Character.ShouldBe(character);
    }

    [Fact]
    public void Location_accessed_through_character()
    {
        var character = new Character("TestHero", Race.Human, CharacterClass.Fighter) { Location = "start" };
        var player = new Player { Id = "player1", Character = character };

        player.Character.Location.ShouldBe("start");

        player.Character.Location = "end";
        player.Character.Location.ShouldBe("end");
    }

    [Fact]
    public void Inventory_accessed_through_character()
    {
        var character = new Character("TestHero", Race.Human, CharacterClass.Fighter) { Location = "start" };
        var player = new Player { Id = "player1", Character = character };

        player.Character.Inventory.IsEmpty.ShouldBeTrue();
    }
}
