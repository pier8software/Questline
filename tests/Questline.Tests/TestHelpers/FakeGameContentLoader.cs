using Questline.Engine.Content;
using Questline.Engine.Content.Data;

namespace Questline.Tests.TestHelpers;

public class FakeGameContentLoader(AdventureContent adventureContent) : IGameContentLoader
{
    public AdventureContent Load(string adventureId) => adventureContent;
}
