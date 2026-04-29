using Questline.Domain.Characters.Entity;
using Shouldly;

namespace Questline.Tests.Domain.Characters.Entity.HitPoints;

public class When_taking_damage
{
    [Fact]
    public void Current_decreases_by_damage_amount()
    {
        var hp = new Questline.Domain.Characters.Entity.HitPoints(max: 8, current: 8);

        hp.Damage(3);

        hp.Current.ShouldBe(5);
        hp.Max.ShouldBe(8);
    }

    [Fact]
    public void Current_clamps_at_zero_when_damage_exceeds_current()
    {
        var hp = new Questline.Domain.Characters.Entity.HitPoints(max: 8, current: 4);

        hp.Damage(10);

        hp.Current.ShouldBe(0);
    }

    [Fact]
    public void Is_alive_when_current_is_positive()
    {
        var hp = new Questline.Domain.Characters.Entity.HitPoints(max: 8, current: 1);

        hp.IsAlive.ShouldBeTrue();
    }

    [Fact]
    public void Is_not_alive_when_current_is_zero()
    {
        var hp = new Questline.Domain.Characters.Entity.HitPoints(max: 8, current: 0);

        hp.IsAlive.ShouldBeFalse();
    }
}
