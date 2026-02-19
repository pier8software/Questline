using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class ExamineCommandHandlerTests
{
    [Fact]
    public void Examine_inventory_item_shows_description()
    {
        var key = new Item { Id = "rusty-key", Name = "rusty key", Description = "An old iron key, its teeth worn by time." };

        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.")
            .BuildState("player1", "chamber");

        state.Player.Character.Inventory.Add(key);
        var handler = new ExamineCommandHandler();

        var result = handler.Handle(state, new Requests.ExamineCommand("rusty key"));

        result.Message.ShouldBe("An old iron key, its teeth worn by time.");
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

        result.Message.ShouldBe("A flickering wooden torch.");
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

        result.Message.ShouldBe("Ancient runes etched into the stone walls.");
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

        result.Message.ShouldBe("Ancient runes etched into the stone walls.");
    }

    [Fact]
    public void Examine_unknown_target_returns_error()
    {
        var state = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.")
            .BuildState("player1", "chamber");

        var handler = new ExamineCommandHandler();

        var result = handler.Handle(state, new Requests.ExamineCommand("mysterious orb"));

        result.Message.ShouldBe("You don't see 'mysterious orb' here.");
    }
}
