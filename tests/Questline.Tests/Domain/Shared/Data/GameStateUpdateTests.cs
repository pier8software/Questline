using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Domain.Shared.Data;

public class GameStateUpdateTests
{
    private static readonly HitPoints DefaultHitPoints = new(8, 8);

    private static readonly AbilityScores DefaultAbilityScores = new(
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10),
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10));

    private static Player CreateTestPlayer(string location = "start") =>
        new("player1", Character.Create("TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, location));

    [Fact]
    public void UpdatePlayer_replaces_current_player()
    {
        var state = new GameState(
            new Dictionary<string, Room>(),
            CreateTestPlayer());

        var newCharacter = state.Player.Character.MoveTo("end");
        var newPlayer = state.Player with { Character = newCharacter };
        state.UpdatePlayer(newPlayer);

        state.Player.Character.Location.ShouldBe("end");
    }

    [Fact]
    public void UpdateRoom_replaces_room_by_id()
    {
        var room = new Room { Id = "cellar", Name = "Cellar", Description = "A damp cellar." };
        var state = new GameState(
            new Dictionary<string, Room> { ["cellar"] = room },
            CreateTestPlayer());

        var updatedRoom = room with { Description = "A very damp cellar." };
        state.UpdateRoom(updatedRoom);

        state.GetRoom("cellar").Description.ShouldBe("A very damp cellar.");
    }

    [Fact]
    public void UpdateBarrier_replaces_barrier_by_id()
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
            CreateTestPlayer(),
            new Dictionary<string, Barrier> { ["iron-door"] = barrier });

        var unlocked = barrier.Unlock();
        state.UpdateBarrier(unlocked);

        state.GetBarrier("iron-door")!.IsUnlocked.ShouldBeTrue();
    }
}
