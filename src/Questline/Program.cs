using Questline.Cli;

var cliApp = await new CliAppBuilder()
    .ConfigureServices()
    .Build();

await cliApp.Run();
