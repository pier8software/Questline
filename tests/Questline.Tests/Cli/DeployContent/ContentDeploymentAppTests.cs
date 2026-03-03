using Questline.Cli.DeployContent;
using Questline.Tests.TestHelpers;

namespace Questline.Tests.Cli.DeployContent;

public class ContentDeploymentAppTests
{
    [Fact]
    public async Task Seeds_content_to_database()
    {
        var seeder = new FakeContentSeeder();
        var console = new FakeConsole();
        var mode = new ContentDeploymentApp(seeder, console);

        await mode.RunAsync();

        seeder.WasCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task Writes_confirmation_message()
    {
        var seeder = new FakeContentSeeder();
        var console = new FakeConsole();
        var mode = new ContentDeploymentApp(seeder, console);

        await mode.RunAsync();

        console.AllOutput.ShouldContain("Content deployed successfully");
    }

    [Fact]
    public async Task Does_not_prompt_for_input()
    {
        var seeder = new FakeContentSeeder();
        var console = new FakeConsole();
        var mode = new ContentDeploymentApp(seeder, console);

        await mode.RunAsync();

        // FakeConsole.ReadLine returns null when no input is queued.
        // If the mode called ReadLine, it would get null — but it should never call it.
        // Verify no prompt character was written.
        console.AllOutput.ShouldNotContain("> ");
    }
}
