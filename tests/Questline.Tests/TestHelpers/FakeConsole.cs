using Questline.Cli;

namespace Questline.Tests.Cli;

public class FakeConsole : IConsole
{
    private readonly Queue<string> _inputs = new();
    private readonly List<string> _output = new();

    public IReadOnlyList<string> Output => _output;

    public string AllOutput => string.Join("", _output);

    public string? ReadLine() => _inputs.Count > 0 ? _inputs.Dequeue() : null;

    public void Write(string text) => _output.Add(text);
    public void WriteLine(string text) => _output.Add(text + "\n");

    public void QueueInput(params string[] lines)
    {
        foreach (var line in lines)
        {
            _inputs.Enqueue(line);
        }
    }
}
