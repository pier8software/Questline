using Questline.Domain.Characters.Entity;
using Shouldly;

namespace Questline.Tests.Domain.Characters.Entity.HitPoints;

public class When_healing
{
    [Fact]
    public void Current_increases_by_heal_amount()
    {
        var hp = new Questline.Domain.Characters.Entity.HitPoints(max: 8, current: 3);

        hp.Heal(2);

        hp.Current.ShouldBe(5);
    }

    [Fact]
    public void Current_clamps_at_max_when_heal_would_exceed_max()
    {
        var hp = new Questline.Domain.Characters.Entity.HitPoints(max: 8, current: 6);

        hp.Heal(10);

        hp.Current.ShouldBe(8);
    }

    [Fact]
    public void Throws_when_heal_amount_is_negative()
    {
        var hp = new Questline.Domain.Characters.Entity.HitPoints(max: 8, current: 4);

        Should.Throw<ArgumentOutOfRangeException>(() => hp.Heal(-1));
    }
}
