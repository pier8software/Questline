namespace Questline.Engine.Core;

public class GameSession : IGameSession
{
    public string? PlaythroughId { get; private set; }

    public void SetPlaythroughId(string id) => PlaythroughId = id;
}
