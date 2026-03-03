using Questline.Domain.Playthroughs.Data;
using Questline.Domain.Playthroughs.Entity;

namespace Questline.Engine.Repositories;

public interface IPlaythroughRepository
{
    Task<Playthrough> GetById(string id);
    Task Save(Playthrough playthrough);
    Task<IReadOnlyList<PlaythroughSummary>> FindByUsername(string username);
}
