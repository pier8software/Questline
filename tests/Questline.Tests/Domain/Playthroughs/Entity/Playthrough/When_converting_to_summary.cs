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
    public void ToPartySummary_returns_one_member_for_default_playthrough()
    {
        var summary = _playthrough.ToPartySummary();

        summary.Members.Count.ShouldBe(1);
    }

    [Fact]
    public void ToPartySummary_leader_has_correct_name()
    {
        var summary = _playthrough.ToPartySummary();

        summary.Members[0].Name.ShouldBe("Aric");
    }

    [Fact]
    public void ToPartySummary_turns_is_zero_initially()
    {
        var summary = _playthrough.ToPartySummary();

        summary.Turns.ShouldBe(0);
    }
}
