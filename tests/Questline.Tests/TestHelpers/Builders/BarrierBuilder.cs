using Questline.Domain.Rooms.Entity;
using TestStack.Dossier;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.TestHelpers.Builders;

public class BarrierBuilder : TestDataBuilder<Barrier, BarrierBuilder>
{
    public BarrierBuilder WithId(string id) =>
        Set(x => x.Id, id);

    public BarrierBuilder WithName(string name) =>
        Set(x => x.Name, name);

    public BarrierBuilder WithDescription(string description) =>
        Set(x => x.Description, description);

    public BarrierBuilder WithBlockedMessage(string blockedMessage) =>
        Set(x => x.BlockedMessage, blockedMessage);

    public BarrierBuilder WithUnlockItemId(string unlockItemId) =>
        Set(x => x.UnlockItemId, unlockItemId);

    public BarrierBuilder WithUnlockMessage(string unlockMessage) =>
        Set(x => x.UnlockMessage, unlockMessage);

    protected override Barrier BuildObject()
    {
        return new Barrier
        {
            Id             = Get(x => x.Id),
            Name           = Get(x => x.Name),
            Description    = Get(x => x.Description),
            BlockedMessage = Get(x => x.BlockedMessage),
            UnlockItemId   = Get(x => x.UnlockItemId),
            UnlockMessage  = Get(x => x.UnlockMessage)
        };
    }
}
