using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Engine.Handlers;

public class ExamineCommandHandlerTests
{
    [Fact]
    public async Task Examine_inventory_item_shows_description()
    {
        var key = new Item
            { Id = "rusty-key", Name = "rusty key", Description = "An old iron key, its teeth worn by time." };

        var fixture = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.")
            .WithInventoryItem(key)
            .Build("chamber");

        var handler = new ExamineCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.ExamineCommand("rusty key"));

        var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
        examineResult.Description.ShouldBe("An old iron key, its teeth worn by time.");
    }

    [Fact]
    public async Task Examine_room_item_shows_description()
    {
        var torch = new Item { Id = "torch", Name = "torch", Description = "A flickering wooden torch." };

        var fixture = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.", r => r.WithItem(torch))
            .Build("chamber");

        var handler = new ExamineCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.ExamineCommand("torch"));

        var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
        examineResult.Description.ShouldBe("A flickering wooden torch.");
    }

    [Fact]
    public async Task Examine_room_feature_by_keyword_shows_description()
    {
        var feature = new Feature
        {
            Id          = "strange-symbols",
            Name        = "strange symbols",
            Keywords    = ["symbols", "carvings"],
            Description = "Ancient runes etched into the stone walls."
        };

        var fixture = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.", r => r.WithFeature(feature))
            .Build("chamber");

        var handler = new ExamineCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.ExamineCommand("symbols"));

        var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
        examineResult.Description.ShouldBe("Ancient runes etched into the stone walls.");
    }

    [Fact]
    public async Task Examine_room_feature_by_name_shows_description()
    {
        var feature = new Feature
        {
            Id          = "strange-symbols",
            Name        = "strange symbols",
            Keywords    = ["symbols", "carvings"],
            Description = "Ancient runes etched into the stone walls."
        };

        var fixture = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.", r => r.WithFeature(feature))
            .Build("chamber");

        var handler = new ExamineCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.ExamineCommand("strange symbols"));

        var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
        examineResult.Description.ShouldBe("Ancient runes etched into the stone walls.");
    }

    [Fact]
    public async Task Examine_unknown_target_returns_error()
    {
        var fixture = new GameBuilder()
            .WithRoom("chamber", "Chamber", "A dark chamber.")
            .Build("chamber");

        var handler = new ExamineCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);

        var result = await handler.Handle(new Requests.ExamineCommand("mysterious orb"));

        var error = result.ShouldBeOfType<ErrorResponse>();
        error.ErrorMessage.ShouldBe("You don't see 'mysterious orb' here.");
    }
}
