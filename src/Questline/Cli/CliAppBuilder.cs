using Microsoft.Extensions.Configuration;
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
    private const string DefaultConnectionString = "mongodb://localhost:27017";

    private readonly ServiceCollection _services = new();
    private IConfiguration _configuration = new ConfigurationBuilder().Build();
    private RunMode _mode = RunMode.Game;

    public CliAppBuilder WithConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
        return this;
    }

    public CliAppBuilder WithRunMode(RunMode mode)
    {
        _mode = mode;
        return this;
    }

    public CliAppBuilder ConfigureServices()
    {
        var connectionString = _configuration.GetConnectionString("questline") ?? DefaultConnectionString;

        _services.AddSingleton<IConsole, SystemConsole>();
        _services.AddMongoPersistence(connectionString, "questline");

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

    public ServiceProvider BuildServiceProvider() => _services.BuildServiceProvider();
}
