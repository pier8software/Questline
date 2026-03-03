using Microsoft.Extensions.Configuration;
using Questline.Cli;

try
{
    var mode = RunModeParser.Parse(args);

    var configuration = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

    var runMode = new CliAppBuilder()
        .WithConfiguration(configuration)
        .WithRunMode(mode)
        .ConfigureServices()
        .Build();

    await runMode.RunAsync();
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine(ex.Message);
    return 1;
}

return 0;
