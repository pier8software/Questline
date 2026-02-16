using Microsoft.Extensions.DependencyInjection;
using Questline.Engine;

namespace Questline.Cli;

public class CliAppBuilder
{
    private readonly ServiceCollection _services = new();
    //private IConfiguration _configuration;

    public CliAppBuilder ConfigureServices()
    {
        _services.AddQuestlineEngine();
        return this;
    }

    //public CliAppBuilder ConfigureApplication() => this;

    public CliApp Build()
    {
        var provider = _services.BuildServiceProvider();
        var gameLoop = provider.GetRequiredService<GameLoop>();

        return new CliApp(gameLoop);
    }
}
