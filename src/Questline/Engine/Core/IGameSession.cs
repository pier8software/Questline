namespace Questline.Engine.Core;

public interface IGameSession
{
    string? PlaythroughId { get; }
    string? Username { get; }
    void SetPlaythroughId(string id);
    void SetUsername(string username);
}
