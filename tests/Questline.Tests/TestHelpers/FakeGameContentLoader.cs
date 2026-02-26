using Questline.Engine.Content;

namespace Questline.Tests.TestHelpers;

public class FakeGameContentLoader(AdventureContent adventureContent) : IGameContentLoader
{
    public AdventureContent Load(string adventureId) => adventureContent;
}
