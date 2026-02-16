using Questline.Domain.Entities;

namespace Questline.Tests.Domain.Entities;

public class ExitTests
{
    [Fact]
    public void BarrierId_defaults_to_null()
    {
        var exit = new Exit("hallway");

        exit.Destination.ShouldBe("hallway");
        exit.BarrierId.ShouldBeNull();
    }

    [Fact]
    public void Exits_with_same_values_are_equal()
    {
        var a = new Exit("hallway", "iron-door");
        var b = new Exit("hallway", "iron-door");

        a.ShouldBe(b);
    }

    [Fact]
    public void Exits_with_different_destinations_are_not_equal()
    {
        var a = new Exit("hallway");
        var b = new Exit("throne-room");

        a.ShouldNotBe(b);
    }

    [Fact]
    public void Exits_with_different_barriers_are_not_equal()
    {
        var a = new Exit("hallway", "iron-door");
        var b = new Exit("hallway", "wooden-door");

        a.ShouldNotBe(b);
    }
}
