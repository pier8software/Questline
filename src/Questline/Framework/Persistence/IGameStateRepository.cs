using Questline.Domain.Shared.Data;

namespace Questline.Framework.Persistence;

public interface IGameStateRepository
{
    void Save(GameState state);
}
