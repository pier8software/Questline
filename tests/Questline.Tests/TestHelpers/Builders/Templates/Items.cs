namespace Questline.Tests.TestHelpers.Builders.Templates;

public static partial class Templates
{
    public static class Items
    {
        public static ItemBuilder BrassLamp =>
            new ItemBuilder()
                .WithId("lamp")
                .WithName("brass lamp")
                .WithDescription("A shiny brass lamp.");

        public static ItemBuilder RustyKey =>
            new ItemBuilder()
                .WithId("rusty-key")
                .WithName("rusty key")
                .WithDescription("An old iron key, its teeth worn by time.");

        public static ItemBuilder Torch =>
            new ItemBuilder()
                .WithId("torch")
                .WithName("torch")
                .WithDescription("A flickering wooden torch.");
    }
}
