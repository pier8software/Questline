using Questline.Engine.Handlers;
using Questline.Engine.Messages;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Engine.Handlers;

public class When_examining_room_feature
{
    private readonly ExamineCommandHandler _handler;

    public When_examining_room_feature()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber.WithFeature(Features.StrangeSymbols))
            .Build("chamber");

        _handler = new ExamineCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
    }

    [Fact]
    public async Task Keyword_shows_description()
    {
        var result = await _handler.Handle(new Requests.ExamineCommand("symbols"));

        var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
        examineResult.Description.ShouldBe("Ancient runes etched into the stone walls.");
    }

    [Fact]
    public async Task Full_name_shows_description()
    {
        var result = await _handler.Handle(new Requests.ExamineCommand("strange symbols"));

        var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
        examineResult.Description.ShouldBe("Ancient runes etched into the stone walls.");
    }
}
