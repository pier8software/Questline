using Questline.Domain.Characters.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Content;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Domain.Shared.Data;

public class AdventureContentUpdateTests
{
    [Fact]
    public void Room_mutation_is_visible_through_adventure_content()
    {
        var lamp = new Item { Id = "lamp", Name   = "brass lamp", Description = "A shiny brass lamp." };
        var room = new Room { Id = "cellar", Name = "Cellar", Description     = "A damp cellar." };
        var adventure = new AdventureContent(
            new Dictionary<string, Room> { ["cellar"] = room },
            new Dictionary<string, Barrier>(),
            "cellar");

        adventure.GetRoom("cellar").AddItem(lamp);

        adventure.GetRoom("cellar").FindItemByName("brass lamp").ShouldBe(lamp);
    }

    [Fact]
    public void Barrier_mutation_is_visible_through_adventure_content()
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

        adventure.GetBarrier("iron-door")!.Unlock();

        adventure.GetBarrier("iron-door")!.IsUnlocked.ShouldBeTrue();
    }
}
