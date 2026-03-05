namespace Questline.Tests.TestHelpers.Builders.Templates;

public static partial class Templates
{
    public static class Barriers
    {
        public static BarrierBuilder IronDoor =>
            new BarrierBuilder()
                .WithId("iron-door")
                .WithName("iron door")
                .WithDescription("A heavy iron door blocks the way North.")
                .WithBlockedMessage("The iron door is locked tight.")
                .WithUnlockItemId("rusty-key")
                .WithUnlockMessage("The rusty key turns in the lock and the iron door swings open.");
    }
}
