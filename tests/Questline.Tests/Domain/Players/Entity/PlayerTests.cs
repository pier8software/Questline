using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;

namespace Questline.Tests.Domain.Players.Entity;

public class PlayerTests
{
    [Fact]
    public void Location_is_mutable()
    {
        var player = new Player { Id = "player1", Character = new Character("TestHero", Race.Human, CharacterClass.Fighter), Location = "start" };
        player.Location.ShouldBe("start");

        player.Location = "end";
        player.Location.ShouldBe("end");
    }

    [Fact]
    public void Inventory_starts_empty()
    {
        var player = new Player { Id = "player1", Character = new Character("TestHero", Race.Human, CharacterClass.Fighter), Location = "start" };

        player.Inventory.IsEmpty.ShouldBeTrue();
    }
}
