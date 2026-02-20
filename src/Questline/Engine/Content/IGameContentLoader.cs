namespace Questline.Engine.Content;

public interface IGameContentLoader
{
    WorldContent Load(string adventureId);
}
