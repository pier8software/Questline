using Questline.Domain.Playthroughs.Entity;
using Questline.Tests.TestHelpers.Builders;
using static Questline.Tests.TestHelpers.Builders.Templates;

namespace Questline.Tests.Domain.Playthroughs.Entity;

public class When_recording_room_items
{
    private readonly Playthrough _playthrough;

    public When_recording_room_items()
    {
        _playthrough = new PlaythroughBuilder().Build();
    }

    [Fact]
    public void GetRecordedRoomItems_returns_null_for_unmodified_room()
    {
        _playthrough.GetRecordedRoomItems("cellar").ShouldBeNull();
    }

    [Fact]
    public void RecordRoomItems_stores_items_for_room()
    {
        var lamp = Items.BrassLamp.Build();

        _playthrough.RecordRoomItems("cellar", [lamp]);

        var items = _playthrough.GetRecordedRoomItems("cellar");
        items.ShouldNotBeNull();
        items!.ShouldContain(lamp);
    }
}
