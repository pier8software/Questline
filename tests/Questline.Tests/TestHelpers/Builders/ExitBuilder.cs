using Questline.Domain.Rooms.Entity;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.TestHelpers.Builders;

public class ExitBuilder
{
    private string   _destination = "default-destination";
    private Barrier? _barrier;

    public ExitBuilder WithDestination(string destination)
    {
        _destination = destination;
        return this;
    }

    public ExitBuilder WithBarrier(Barrier barrier)
    {
        _barrier = barrier;
        return this;
    }

    public ExitBuilder WithBarrier(BarrierBuilder barrierBuilder)
    {
        _barrier = barrierBuilder.Build();
        return this;
    }

    public Exit Build() => new(_destination, _barrier);
}
