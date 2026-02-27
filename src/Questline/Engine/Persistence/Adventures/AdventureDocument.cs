using Questline.Framework.Persistence;

namespace Questline.Engine.Persistence.Adventures;

public class AdventureDocument : Document
{
    public string Name           { get; set; } = null!;
    public string Description    { get; set; } = null!;
    public string StartingRoomId { get; set; } = null!;
}
