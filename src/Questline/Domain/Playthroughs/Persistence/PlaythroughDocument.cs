using Questline.Domain.Adventures.Persistence;
using Questline.Framework.Persistence;

namespace Questline.Domain.Playthroughs.Persistence;

public class PlaythroughDocument : Document
{
    public string PlayerId    { get; set; } = null!;
    public string CharacterId { get; set; } = null!;
    public AdventureDocument Adventure { get; set; } = null!;
    public DateTimeOffset    StartedAt { get; set; }
}
