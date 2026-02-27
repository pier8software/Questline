namespace Questline.Engine.Core;

public interface IGameSession
{
    string? PlaythroughId { get; }
    void SetPlaythroughId(string id);
}
