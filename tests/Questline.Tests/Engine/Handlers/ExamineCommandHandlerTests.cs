using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class ExamineCommandHandlerTests
{
    private static void GiveItemToPlayer(GameState state, Item item) => state.Player.Character.AddInventoryItem(item);

    [Fact]
    public void Examine_inventory_item_shows_description()
    {
        var key = new Item
            { Id = "rusty-key", Name = "rusty key", Description = "An old iron key, its teeth worn by time." };

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.")
            .BuildState("player1", "chamber");

        GiveItemToPlayer(state, key);
        var handler = new ExamineCommandHandler();

        var result = handler.Handle(state, new Requests.ExamineCommand("rusty key"));

        var examine = result.ShouldBeOfType<Responses.ExamineResponse>();
        examine.Description.ShouldBe("An old iron key, its teeth worn by time.");
    }

    [Fact]
    public void Examine_room_item_shows_description()
    {
        var torch = new Item { Id = "torch", Name = "torch", Description = "A flickering wooden torch." };

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.", r => r.WithItem(torch))
            .BuildState("player1", "chamber");

        var handler = new ExamineCommandHandler();

        var result = handler.Handle(state, new Requests.ExamineCommand("torch"));

        var examine = result.ShouldBeOfType<Responses.ExamineResponse>();
        examine.Description.ShouldBe("A flickering wooden torch.");
    }

    [Fact]
    public void Examine_room_feature_by_keyword_shows_description()
    {
        var feature = new Feature
        {
            Id = "strange-symbols",
            Name = "strange symbols",
            Keywords = ["symbols", "carvings"],
            Description = "Ancient runes etched into the stone walls."
        };

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.", r => r.WithFeature(feature))
            .BuildState("player1", "chamber");

        var handler = new ExamineCommandHandler();

        var result = handler.Handle(state, new Requests.ExamineCommand("symbols"));

        var examine = result.ShouldBeOfType<Responses.ExamineResponse>();
        examine.Description.ShouldBe("Ancient runes etched into the stone walls.");
    }

    [Fact]
    public void Examine_room_feature_by_name_shows_description()
    {
        var feature = new Feature
        {
            Id = "strange-symbols",
            Name = "strange symbols",
            Keywords = ["symbols", "carvings"],
            Description = "Ancient runes etched into the stone walls."
        };

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.", r => r.WithFeature(feature))
            .BuildState("player1", "chamber");

        var handler = new ExamineCommandHandler();

        var result = handler.Handle(state, new Requests.ExamineCommand("strange symbols"));

        var examine = result.ShouldBeOfType<Responses.ExamineResponse>();
        examine.Description.ShouldBe("Ancient runes etched into the stone walls.");
    }

    [Fact]
    public void Examine_unknown_target_returns_error()
    {
        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.")
            .BuildState("player1", "chamber");

        var handler = new ExamineCommandHandler();

        var result = handler.Handle(state, new Requests.ExamineCommand("mysterious orb"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("You don't see 'mysterious orb' here.");
    }
}
