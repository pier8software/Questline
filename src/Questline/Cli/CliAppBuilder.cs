using Microsoft.Extensions.DependencyInjection;
using Questline.Engine;
using Questline.Framework.Persistence;

namespace Questline.Cli;

public class CliAppBuilder
{
    private readonly ServiceCollection _services = new();

    public CliAppBuilder ConfigureServices()
    {
        _services.AddSingleton<IConsole, SystemConsole>();
        _services.AddSingleton<CliApp>();

        _services.AddQuestlineEngine();
        _services.AddMongoPersistence("mongodb://localhost:27017", "questline", "0.6.0");
        return this;
    }

    public CliApp Build()
    {
        var provider = _services.BuildServiceProvider();

        return provider.GetRequiredService<CliApp>();
    }
}
