using Questline.Cli;
using Terminal.Gui;

Application.Init();
try
{
    var parser = new CommandParser();
    var handler = new CommandHandler();
    var gameWindow = new GameWindow(parser, handler);
    Application.Run(gameWindow);
    gameWindow.Dispose();
}
finally
{
    Application.Shutdown();
}
