using Questline.Domain.Playthroughs.Entity;

namespace Questline.Engine.Repositories;

public class InMemoryPlaythroughRepository : IPlaythroughRepository
{
    private readonly Dictionary<string, Playthrough> _store = new();

    public Task<Playthrough> GetById(string id)
    {
        if (!_store.TryGetValue(id, out var playthrough))
        {
            throw new KeyNotFoundException($"Playthrough '{id}' not found.");
        }

        return Task.FromResult(playthrough);
    }

    public Task Save(Playthrough playthrough)
    {
        _store[playthrough.Id] = playthrough;
        return Task.CompletedTask;
    }
}
