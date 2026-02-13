using Questline.Domain;
using Questline.Domain.Entities;
using Questline.Domain.Shared;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Domain;

public class GameStateTests
{
    [Fact]
    public void Holds_world_and_player()
    {
        var world = new GameBuilder()
            .WithRoom("start", "Start", "Starting room.")
            .Build();
        var player = new Player { Id = "player1", Location = "start" };

        var state = new GameState(world, player);

        state.ShouldBeSameAs(world);
        state.Player.ShouldBeSameAs(player);
    }
}
