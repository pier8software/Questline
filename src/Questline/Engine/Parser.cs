namespace Questline.Engine;

public class Parser
{
    public ParsedInput Parse(string input)
    {
        var tokens = input.Trim().ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length == 0)
        {
            return new ParsedInput("", []);
        }

        return new ParsedInput(tokens[0], tokens[1..]);
    }
}
