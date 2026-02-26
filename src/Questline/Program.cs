using Questline.Cli;

var cliApp = new CliAppBuilder()
    .ConfigureServices()
    .Build();

await cliApp.Run();
