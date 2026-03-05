using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Domain.Playthroughs.Entity.Playthrough;

public class When_interacting_with_barriers
{
    private readonly Questline.Domain.Playthroughs.Entity.Playthrough _playthrough;

    public When_interacting_with_barriers()
    {
        _playthrough = new PlaythroughBuilder().Build();
    }

    [Fact]
    public void Barrier_starts_locked()
    {
        _playthrough.IsBarrierUnlocked("iron-door").ShouldBeFalse();
    }

    [Fact]
    public void Barrier_is_unlocked_after_unlock_call()
    {
        _playthrough.UnlockBarrier("iron-door");

        _playthrough.IsBarrierUnlocked("iron-door").ShouldBeTrue();
    }
}
