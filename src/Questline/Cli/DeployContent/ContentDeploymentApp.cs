using Questline.Engine.Content;

namespace Questline.Cli.DeployContent;

public class ContentDeploymentApp(IContentSeeder seeder, IConsole console) : ICliApp
{
    public async Task RunAsync()
    {
        await seeder.SeedAdventure("the-goblins-lair.json");
        console.WriteLine("Content deployed successfully.");
    }
}
