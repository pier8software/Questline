using Questline.Domain;

namespace Questline.Tests.Domain;

public class GameStateTests
{
    [Fact]
    public void Holds_world_and_player()
    {
        var world = new WorldBuilder()
            .WithRoom("start", "Start", "Starting room.")
            .Build();
        var player = new Player { Id = "player1", Location = "start" };

        var state = new GameState(world, player);

        state.World.ShouldBeSameAs(world);
        state.Player.ShouldBeSameAs(player);
    }
}
