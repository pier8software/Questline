using Questline.Domain.Rooms.Entity;
using Questline.Engine.Content;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Domain.Shared.Data;

public class AdventureContentBarrierTests
{
    [Fact]
    public void GetBarrier_returns_barrier_when_found()
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

        var adventure = new AdventureContent(
            new Dictionary<string, Room>(),
            new Dictionary<string, Barrier> { ["iron-door"] = barrier },
            "start");

        adventure.GetBarrier("iron-door").ShouldBe(barrier);
    }

    [Fact]
    public void GetBarrier_returns_null_when_not_found()
    {
        var adventure = new AdventureContent(
            new Dictionary<string, Room>(),
            new Dictionary<string, Barrier>(),
            "start");

        adventure.GetBarrier("nonexistent").ShouldBeNull();
    }

    [Fact]
    public void GetBarrier_returns_null_when_id_is_null()
    {
        var adventure = new AdventureContent(
            new Dictionary<string, Room>(),
            new Dictionary<string, Barrier>(),
            "start");

        adventure.GetBarrier(null).ShouldBeNull();
    }
}
