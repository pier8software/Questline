using Questline.Domain.Playthroughs.Entity;

namespace Questline.Engine.Repositories;

public interface IPlaythroughRepository
{
    Task<Playthrough> GetById(string id);
    Task Save(Playthrough playthrough);
}
