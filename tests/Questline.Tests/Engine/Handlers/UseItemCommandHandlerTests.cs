using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Engine.Handlers;

public class UseItemCommandHandlerTests
{
    private static Barrier CreateBarrier() => new()
    {
        Id = "iron-door",
        Name = "iron door",
        Description = "A heavy iron door blocks the way North.",
        BlockedMessage = "The iron door is locked tight.",
        UnlockItemId = "rusty-key",
        UnlockMessage = "The rusty key turns in the lock and the iron door swings open."
    };

    [Fact]
    public void Correct_item_on_barrier_unlocks_it()
    {
        var barrier = CreateBarrier();
        var key = new Item { Id = "rusty-key", Name = "rusty key", Description = "An old iron key." };

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.",
                r => r.WithExit(Direction.North, new Exit("beyond", "iron-door")))
            .WithRoom("beyond", "Beyond", "Beyond the door.")
            .WithBarrier(barrier)
            .BuildState("player1", "chamber");

        state.Player.Inventory.Add(key);
        var handler = new UseItemCommandHandler();

        var result = handler.Handle(state, new Requests.UseItemCommand("rusty key", "iron door"));

        result.Message.ShouldBe("The rusty key turns in the lock and the iron door swings open.");
        barrier.IsUnlocked.ShouldBeTrue();
    }

    [Fact]
    public void Wrong_item_returns_error_and_barrier_stays_locked()
    {
        var barrier = CreateBarrier();
        var torch = new Item { Id = "torch", Name = "torch", Description = "A flickering torch." };

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.",
                r => r.WithExit(Direction.North, new Exit("beyond", "iron-door")))
            .WithRoom("beyond", "Beyond", "Beyond the door.")
            .WithBarrier(barrier)
            .BuildState("player1", "chamber");

        state.Player.Inventory.Add(torch);
        var handler = new UseItemCommandHandler();

        var result = handler.Handle(state, new Requests.UseItemCommand("torch", "iron door"));

        result.Message.ShouldBe("The torch doesn't work on the iron door.");
        barrier.IsUnlocked.ShouldBeFalse();
    }

    [Fact]
    public void Item_not_in_inventory_returns_error()
    {
        var barrier = CreateBarrier();

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.",
                r => r.WithExit(Direction.North, new Exit("beyond", "iron-door")))
            .WithRoom("beyond", "Beyond", "Beyond the door.")
            .WithBarrier(barrier)
            .BuildState("player1", "chamber");

        var handler = new UseItemCommandHandler();

        var result = handler.Handle(state, new Requests.UseItemCommand("rusty key", "iron door"));

        result.Message.ShouldBe("You don't have 'rusty key'.");
    }

    [Fact]
    public void Contextual_use_unlocks_matching_barrier_in_room()
    {
        var barrier = CreateBarrier();
        var key = new Item { Id = "rusty-key", Name = "rusty key", Description = "An old iron key." };

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.",
                r => r.WithExit(Direction.North, new Exit("beyond", "iron-door")))
            .WithRoom("beyond", "Beyond", "Beyond the door.")
            .WithBarrier(barrier)
            .BuildState("player1", "chamber");

        state.Player.Inventory.Add(key);
        var handler = new UseItemCommandHandler();

        var result = handler.Handle(state, new Requests.UseItemCommand("rusty key", null));

        result.Message.ShouldBe("The rusty key turns in the lock and the iron door swings open.");
        barrier.IsUnlocked.ShouldBeTrue();
    }

    [Fact]
    public void Target_not_found_returns_error()
    {
        var key = new Item { Id = "rusty-key", Name = "rusty key", Description = "An old iron key." };

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.", r => r.WithExit(Direction.North, "beyond"))
            .WithRoom("beyond", "Beyond", "Beyond the door.")
            .BuildState("player1", "chamber");

        state.Player.Inventory.Add(key);
        var handler = new UseItemCommandHandler();

        var result = handler.Handle(state, new Requests.UseItemCommand("rusty key", "iron door"));

        result.Message.ShouldBe("You don't see 'iron door' here.");
    }

    [Fact]
    public void Already_unlocked_barrier_returns_informative_message()
    {
        var barrier = CreateBarrier();
        barrier.IsUnlocked = true;
        var key = new Item { Id = "rusty-key", Name = "rusty key", Description = "An old iron key." };

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.",
                r => r.WithExit(Direction.North, new Exit("beyond", "iron-door")))
            .WithRoom("beyond", "Beyond", "Beyond the door.")
            .WithBarrier(barrier)
            .BuildState("player1", "chamber");

        state.Player.Inventory.Add(key);
        var handler = new UseItemCommandHandler();

        var result = handler.Handle(state, new Requests.UseItemCommand("rusty key", "iron door"));

        result.Message.ShouldBe("The iron door is already unlocked.");
    }
}
