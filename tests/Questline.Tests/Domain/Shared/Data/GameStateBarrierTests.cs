using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Domain.Shared.Data;

public class GameStateBarrierTests
{
    [Fact]
    public void GetBarrier_returns_barrier_when_found()
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

        var state = new GameState(
            new Dictionary<string, Room>(),
            new Player { Id = "player1", Character = new Character("TestHero", Race.Human, CharacterClass.Fighter), Location = "start" },
            new Dictionary<string, Barrier> { ["iron-door"] = barrier });

        state.GetBarrier("iron-door").ShouldBe(barrier);
    }

    [Fact]
    public void GetBarrier_returns_null_when_not_found()
    {
        var state = new GameState(
            new Dictionary<string, Room>(),
            new Player { Id = "player1", Character = new Character("TestHero", Race.Human, CharacterClass.Fighter), Location = "start" });

        state.GetBarrier("nonexistent").ShouldBeNull();
    }

    [Fact]
    public void GetBarrier_returns_null_when_id_is_null()
    {
        var state = new GameState(
            new Dictionary<string, Room>(),
            new Player { Id = "player1", Character = new Character("TestHero", Race.Human, CharacterClass.Fighter), Location = "start" });

        state.GetBarrier(null).ShouldBeNull();
    }
}
