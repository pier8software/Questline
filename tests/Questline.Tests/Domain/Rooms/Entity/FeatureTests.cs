using Questline.Domain.Rooms.Entity;

namespace Questline.Tests.Domain.Rooms.Entity;

public class FeatureTests
{
    [Fact]
    public void Feature_has_properties()
    {
        var feature = new Feature
        {
            Id = "strange-symbols",
            Name = "strange symbols",
            Keywords = ["symbols", "carvings"],
            Description = "Ancient runes etched into the stone."
        };

        feature.Id.ShouldBe("strange-symbols");
        feature.Name.ShouldBe("strange symbols");
        feature.Keywords.ShouldBe(["symbols", "carvings"]);
        feature.Description.ShouldBe("Ancient runes etched into the stone.");
    }
}
