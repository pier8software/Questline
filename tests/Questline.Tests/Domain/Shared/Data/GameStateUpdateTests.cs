using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
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
    public void Character_mutation_is_visible_through_game_state()
    {
        var state = new GameState(
            new Dictionary<string, Room>());

        state.Player.Character.MoveTo("end");

        state.Player.Character.Location.ShouldBe("end");
    }

    [Fact]
    public void Room_mutation_is_visible_through_game_state()
    {
        var lamp = new Item { Id = "lamp", Name   = "brass lamp", Description = "A shiny brass lamp." };
        var room = new Room { Id = "cellar", Name = "Cellar", Description     = "A damp cellar." };
        var state = new GameState(
            new Dictionary<string, Room> { ["cellar"] = room });

        state.GetRoom("cellar").AddItem(lamp);

        state.GetRoom("cellar").FindItemByName("brass lamp").ShouldBe(lamp);
    }

    [Fact]
    public void Barrier_mutation_is_visible_through_game_state()
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

        var state = new GameState(
            new Dictionary<string, Room>(),
            new Dictionary<string, Barrier> { ["iron-door"] = barrier });

        state.GetBarrier("iron-door")!.Unlock();

        state.GetBarrier("iron-door")!.IsUnlocked.ShouldBeTrue();
    }
}
