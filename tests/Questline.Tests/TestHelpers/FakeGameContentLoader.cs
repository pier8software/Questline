using Questline.Engine.Content;

namespace Questline.Tests.TestHelpers;

public class FakeGameContentLoader(WorldContent worldContent) : IGameContentLoader
{
    public WorldContent Load(string adventureId) => worldContent;
}
