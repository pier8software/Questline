namespace Questline.Engine.Content;

public interface IGameContentLoader
{
    AdventureContent Load(string adventureId);
}
