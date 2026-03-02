using Questline.Cli;

try
{
    var mode = RunModeParser.Parse(args);

    var runMode = new CliAppBuilder()
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
