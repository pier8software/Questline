using Questline.Domain;
using Questline.Domain.Entities;

namespace Questline.Tests.Domain;

public class PlayerTests
{
    [Fact]
    public void Location_is_mutable()
    {
        var player = new Player { Id = "player1", Location = "start" };
        player.Location.ShouldBe("start");

        player.Location = "end";
        player.Location.ShouldBe("end");
    }

    [Fact]
    public void Inventory_starts_empty()
    {
        var player = new Player { Id = "player1", Location = "start" };

        player.Inventory.IsEmpty.ShouldBeTrue();
    }
}
