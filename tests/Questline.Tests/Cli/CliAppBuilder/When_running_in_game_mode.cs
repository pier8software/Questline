using Microsoft.Extensions.DependencyInjection;
using Questline.Cli;
using Questline.Engine.Content;
using Questline.Framework.FileSystem;

namespace Questline.Tests.Cli.CliAppBuilder;

public class When_running_in_game_mode
{
    private readonly ServiceProvider _provider;

    public When_running_in_game_mode()
    {
        var builder = new Questline.Cli.CliAppBuilder()
            .WithRunMode(RunMode.Game)
            .ConfigureServices();

        _provider = builder.BuildServiceProvider();
    }

    [Fact]
    public void Does_not_register_content_seeder()
    {
        _provider.GetService<IContentSeeder>().ShouldBeNull();
    }

    [Fact]
    public void Does_not_register_json_file_loader()
    {
        _provider.GetService<JsonFileLoader>().ShouldBeNull();
    }
}
