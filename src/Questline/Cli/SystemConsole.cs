namespace Questline.Cli;

public class SystemConsole : IConsole
{
    public string? ReadLine()             => Console.ReadLine();
    public void    Write(string     text) => Console.Write(text);
    public void    WriteLine(string text) => Console.WriteLine(text);
}
