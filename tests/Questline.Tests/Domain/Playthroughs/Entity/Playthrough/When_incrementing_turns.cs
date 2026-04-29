using Questline.Tests.TestHelpers.Builders;
using Shouldly;

namespace Questline.Tests.Domain.Playthroughs.Entity.Playthrough;

public class When_incrementing_turns
{
    [Fact]
    public void Starts_at_zero()
    {
        var playthrough = PlaythroughBuilder.New().Build();

        playthrough.Turns.ShouldBe(0);
    }

    [Fact]
    public void Each_increment_advances_by_one()
    {
        var playthrough = PlaythroughBuilder.New().Build();

        playthrough.IncrementTurns();
        playthrough.IncrementTurns();
        playthrough.IncrementTurns();

        playthrough.Turns.ShouldBe(3);
    }
}
