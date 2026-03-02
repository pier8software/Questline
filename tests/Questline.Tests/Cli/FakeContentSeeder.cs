using Questline.Engine.Content;

namespace Questline.Tests.Cli;

public class FakeContentSeeder : IContentSeeder
{
    public bool WasCalled { get; private set; }
    public string? SeededFilePath { get; private set; }

    public Task SeedAdventure(string filePath, CancellationToken cancellationToken = default)
    {
        WasCalled = true;
        SeededFilePath = filePath;
        return Task.CompletedTask;
    }
}
