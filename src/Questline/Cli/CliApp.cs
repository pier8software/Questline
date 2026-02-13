namespace Questline.Cli;

public class CliApp(GameLoop gameLoop)
{
    public void Run() => gameLoop.Run();
}
