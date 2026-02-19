using Microsoft.Extensions.DependencyInjection;
using Questline.Engine;

namespace Questline.Cli;

public class CliAppBuilder
{
    private readonly ServiceCollection _services = new();

    public CliAppBuilder ConfigureServices()
    {
        _services.AddSingleton<IConsole, SystemConsole>();
        _services.AddSingleton<CliApp>();

        _services.AddQuestlineEngine();
        return this;
    }

    public CliApp Build()
    {
        var provider = _services.BuildServiceProvider();

        return provider.GetRequiredService<CliApp>();
    }
}
