using Questline.Domain.Players.Entity;
using Questline.Domain.Shared.Data;
using Questline.Framework.Persistence;

namespace Questline.Tests.TestHelpers;

public class FakeGameStateRepository : IGameStateRepository
{
    public List<SaveSnapshot> SaveCalls { get; } = [];

    public void Save(GameState state) => SaveCalls.Add(new SaveSnapshot(state.Player));

    public record SaveSnapshot(Player? Player);
}
