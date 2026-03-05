using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Domain.Playthroughs.Entity.Playthrough;

public class When_converting_playthrough_to_summary
{
    private readonly Questline.Domain.Playthroughs.Entity.Playthrough _playthrough;

    public When_converting_playthrough_to_summary()
    {
        _playthrough = new PlaythroughBuilder().Build();
    }

    [Fact]
    public void ToCharacterSummary_returns_summary()
    {
        var summary = _playthrough.ToCharacterSummary();

        summary.Name.ShouldBe("TestHero");
        summary.Race.ShouldBe("Human");
        summary.Class.ShouldBe("Fighter");
        summary.Level.ShouldBe(1);
    }
}
