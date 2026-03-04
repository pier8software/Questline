namespace Questline.Tests.TestHelpers.Builders;

public static partial class Templates
{
    public static class Features
    {
        public static FeatureBuilder StrangeSymbols =>
            new FeatureBuilder()
                .WithId("strange-symbols")
                .WithName("strange symbols")
                .WithKeywords(["symbols", "carvings"])
                .WithDescription("Ancient runes etched into the stone walls.");
    }
}
