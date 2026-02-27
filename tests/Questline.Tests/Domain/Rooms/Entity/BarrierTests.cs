using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Domain.Rooms.Entity;

public class BarrierTests
{
    [Fact]
    public void Barrier_stores_all_content_properties()
    {
        var barrier = new Barrier
        {
            Id             = "iron-door",
            Name           = "iron door",
            Description    = "A heavy iron door.",
            BlockedMessage = "The iron door is locked tight.",
            UnlockItemId   = "rusty-key",
            UnlockMessage  = "The rusty key turns in the lock..."
        };

        barrier.Id.ShouldBe("iron-door");
        barrier.Name.ShouldBe("iron door");
        barrier.Description.ShouldBe("A heavy iron door.");
        barrier.BlockedMessage.ShouldBe("The iron door is locked tight.");
        barrier.UnlockItemId.ShouldBe("rusty-key");
        barrier.UnlockMessage.ShouldBe("The rusty key turns in the lock...");
    }
}
