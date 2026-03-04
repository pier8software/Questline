using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Questline.Cli;

namespace Questline.Tests.Cli;

public class When_configuring_connection_string
{
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
