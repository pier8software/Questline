namespace Questline.Engine.Core;

public class GameSession : IGameSession
{
    public string? PlaythroughId { get; private set; }
    public string? Username { get; private set; }

    public void SetPlaythroughId(string id) => PlaythroughId = id;
    public void SetUsername(string username) => Username = username;
}
