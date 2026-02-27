using Microsoft.Extensions.DependencyInjection;
using Questline.Engine;
using Questline.Engine.Content;
using Questline.Framework.Persistence;

namespace Questline.Cli;

public class CliAppBuilder
{
    private readonly ServiceCollection _services = new();

    public CliAppBuilder ConfigureServices()
    {
        _services.AddSingleton<IConsole, SystemConsole>();
        _services.AddSingleton<ResponseFormatter>();
        _services.AddSingleton<CliApp>();

        _services.AddMongoPersistence("mongodb://localhost:27017", "questline");
        _services.AddQuestlineEngine();
        return this;
    }

    public async Task<CliApp> Build()
    {
        var provider = _services.BuildServiceProvider();

        var seeder = provider.GetRequiredService<ContentSeeder>();
        await seeder.SeedAdventure("the-goblins-lair.json");

        return provider.GetRequiredService<CliApp>();
    }
}
