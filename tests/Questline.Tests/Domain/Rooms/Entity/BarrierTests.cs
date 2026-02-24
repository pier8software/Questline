using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Domain.Rooms.Entity;

public class BarrierTests
{
    [Fact]
    public void Barrier_starts_locked()
    {
        var barrier = new Barrier
        {
            Id = "iron-door",
            Name = "iron door",
            Description = "A heavy iron door.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId = "rusty-key",
            UnlockMessage = "The rusty key turns in the lock..."
        };

        barrier.IsUnlocked.ShouldBeFalse();
    }

    [Fact]
    public void Unlock_returns_new_barrier_with_IsUnlocked_true()
    {
        var barrier = new Barrier
        {
            Id = "iron-door",
            Name = "iron door",
            Description = "A heavy iron door.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId = "rusty-key",
            UnlockMessage = "The rusty key turns in the lock..."
        };

        var unlocked = barrier.Unlock();

        unlocked.IsUnlocked.ShouldBeTrue();
    }

    [Fact]
    public void Unlock_leaves_original_barrier_locked()
    {
        var barrier = new Barrier
        {
            Id = "iron-door",
            Name = "iron door",
            Description = "A heavy iron door.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId = "rusty-key",
            UnlockMessage = "The rusty key turns in the lock..."
        };

        _ = barrier.Unlock();

        barrier.IsUnlocked.ShouldBeFalse();
    }
}
