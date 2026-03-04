using Questline.Domain.Rooms.Entity;
using TestStack.Dossier;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.TestHelpers.Builders;

public class ExitBuilder : TestDataBuilder<Exit, ExitBuilder>
{
    public ExitBuilder WithDestination(string destination) =>
        Set(x => x.Destination, destination);

    public ExitBuilder WithBarrier(Barrier barrier) =>
        Set(x => x.Barrier, barrier);

    public ExitBuilder WithBarrier(BarrierBuilder barrierBuilder) =>
        Set(x => x.Barrier, barrierBuilder);

    protected override Exit BuildObject()
    {
        return new Exit
        {
            Destination = Get(x => x.Destination),
            Barrier     = Get(x => x.Barrier)
        };
    }
}
