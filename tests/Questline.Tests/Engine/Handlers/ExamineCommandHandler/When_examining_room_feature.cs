using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates.Templates;

namespace Questline.Tests.Engine.Handlers.ExamineCommandHandler;

public class When_examining_room_feature
{
    private readonly Questline.Engine.Handlers.ExamineCommandHandler _handler;

    public When_examining_room_feature()
    {
        var fixture = new GameBuilder()
            .WithRoom(Rooms.Chamber.WithFeature(Features.StrangeSymbols))
            .Build("chamber");

        _handler = new Questline.Engine.Handlers.ExamineCommandHandler(
            fixture.Session, fixture.PlaythroughRepository, fixture.RoomRepository);
    }

    [Fact]
    public async Task Keyword_shows_description()
    {
        var result = await _handler.Handle(new PartyActor(), new Requests.ExamineCommand("symbols"));

        var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
        examineResult.Description.ShouldBe("Ancient runes etched into the stone walls.");
    }

    [Fact]
    public async Task Full_name_shows_description()
    {
        var result = await _handler.Handle(new PartyActor(), new Requests.ExamineCommand("strange symbols"));

        var examineResult = result.ShouldBeOfType<Responses.ExamineResponse>();
        examineResult.Description.ShouldBe("Ancient runes etched into the stone walls.");
    }
}
