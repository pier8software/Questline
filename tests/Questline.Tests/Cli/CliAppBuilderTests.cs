using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Questline.Cli;
using Questline.Engine.Content;
using Questline.Framework.FileSystem;

namespace Questline.Tests.Cli;

public class CliAppBuilderTests
{
    [Fact]
    public void Game_mode_does_not_register_content_seeder()
    {
        var builder = new CliAppBuilder()
            .WithRunMode(RunMode.Game)
            .ConfigureServices();

        var provider = builder.BuildServiceProvider();

        provider.GetService<IContentSeeder>().ShouldBeNull();
    }

    [Fact]
    public void Game_mode_does_not_register_json_file_loader()
    {
        var builder = new CliAppBuilder()
            .WithRunMode(RunMode.Game)
            .ConfigureServices();

        var provider = builder.BuildServiceProvider();

        provider.GetService<JsonFileLoader>().ShouldBeNull();
    }

    [Fact]
    public void Uses_connection_string_from_configuration()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:questline"] = "mongodb://custom-host:12345"
            })
            .Build();

        var builder = new CliAppBuilder()
            .WithConfiguration(config)
            .WithRunMode(RunMode.DeployContent)
            .ConfigureServices();

        var provider = builder.BuildServiceProvider();
        var client = provider.GetRequiredService<IMongoClient>();

        client.Settings.Server.Host.ShouldBe("custom-host");
        client.Settings.Server.Port.ShouldBe(12345);
    }

    [Fact]
    public void Falls_back_to_localhost_when_no_configuration()
    {
        var config = new ConfigurationBuilder().Build();

        var builder = new CliAppBuilder()
            .WithConfiguration(config)
            .WithRunMode(RunMode.DeployContent)
            .ConfigureServices();

        var provider = builder.BuildServiceProvider();
        var client = provider.GetRequiredService<IMongoClient>();

        client.Settings.Server.Host.ShouldBe("localhost");
        client.Settings.Server.Port.ShouldBe(27017);
    }
}
