using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;
using Questline.Domain.Shared.Data;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class QuitGameHandlerTests
{
    [Fact]
    public void Returns_quited_response()
    {
        var world = new GameBuilder()
            .WithRoom("tavern", "The Tavern", "A cozy tavern.")
            .Build();

        var state = new GameState(world, new Player { Id = "player1", Character = new Character("TestHero", Race.Human, CharacterClass.Fighter), Location = "tavern" });

        var handler = new QuitGameHandler();

        var result = handler.Handle(state, new Requests.QuitGame());

        result.ShouldBeOfType<Responses.GameQuited>();
        result.Message.ShouldBe("Goodbye!");
    }
}
