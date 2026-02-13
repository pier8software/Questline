using Questline.Cli;


var cliApp = new CliAppBuilder()
    .ConfigureServices()
    .Build();

cliApp.Run();
