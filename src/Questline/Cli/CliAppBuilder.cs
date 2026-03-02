using Microsoft.Extensions.DependencyInjection;
using Questline.Cli.DeployContent;
using Questline.Cli.Game;
using Questline.Engine;
using Questline.Engine.Content;
using Questline.Framework.FileSystem;
using Questline.Framework.Persistence;

namespace Questline.Cli;

public class CliAppBuilder
{
    private readonly ServiceCollection _services = new();
    private RunMode _mode = RunMode.Game;

    public CliAppBuilder WithRunMode(RunMode mode)
    {
        _mode = mode;
        return this;
    }

    public CliAppBuilder ConfigureServices()
    {
        _services.AddSingleton<IConsole, SystemConsole>();
        _services.AddMongoPersistence("mongodb://localhost:27017", "questline");

        switch (_mode)
        {
            case RunMode.Game:
                _services.AddSingleton<ResponseFormatter>();
                _services.AddQuestlineEngine();
                _services.AddSingleton<ICliApp, GameApp>();
                break;

            case RunMode.DeployContent:
                _services.AddSingleton<JsonFileLoader>();
                _services.AddSingleton<IContentSeeder, ContentSeeder>();
                _services.AddSingleton<ICliApp, ContentDeploymentApp>();
                break;
        }

        return this;
    }

    public ICliApp Build()
    {
        var provider = _services.BuildServiceProvider();
        return provider.GetRequiredService<ICliApp>();
    }
}
