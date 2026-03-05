using Questline.Tests.TestHelpers;

namespace Questline.Tests.Cli.DeployContent.ContentDeploymentApp;

public class When_deploying_content
{
    private readonly FakeContentSeeder _seeder;
    private readonly FakeConsole _console;

    public When_deploying_content()
    {
        _seeder = new FakeContentSeeder();
        _console = new FakeConsole();
        var app = new Questline.Cli.DeployContent.ContentDeploymentApp(_seeder, _console);

        app.RunAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public void Seeds_content_to_database()
    {
        _seeder.WasCalled.ShouldBeTrue();
    }

    [Fact]
    public void Writes_confirmation_message()
    {
        _console.AllOutput.ShouldContain("Content deployed successfully");
    }

    [Fact]
    public void Does_not_prompt_for_input()
    {
        _console.AllOutput.ShouldNotContain("> ");
    }
}
