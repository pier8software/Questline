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
    public void Can_Unlock_a_barrier()
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

        barrier = barrier.Unlock();

        barrier.IsUnlocked.ShouldBeTrue();
    }
}
