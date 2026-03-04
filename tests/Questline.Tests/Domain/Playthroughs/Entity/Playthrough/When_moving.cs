using Questline.Domain.Playthroughs.Entity;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Domain.Playthroughs.Entity;

public class When_moving
{
    private readonly Playthrough _playthrough;

    public When_moving()
    {
        _playthrough = new PlaythroughBuilder().Build();
    }

    [Fact]
    public void MoveTo_updates_location()
    {
        _playthrough.MoveTo("end");

        _playthrough.Location.ShouldBe("end");
    }
}
