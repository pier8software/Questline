namespace Questline.Tests.TestHelpers.Builders.Templates;

public static class Exits
{
    public static ExitBuilder Default => new();

    public static ExitBuilder WithBarrier =>
        Default.WithBarrier(Barriers.IronDoor);
}
