using OneOf;
using Questline.Framework.Mediator;

namespace Questline.Engine.InputParsers;

public class Parser(Dictionary<string, Func<string[], OneOf<ICommand, ParseError>>> parsers)
{
    public OneOf<ICommand, ParseError> Parse(string input)
    {
        var tokens = input.Trim().ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length == 0)
        {
            return new ParseError("Please enter a command.");
        }

        return !parsers.TryGetValue(tokens[0], out var parser)
            ? (OneOf<ICommand, ParseError>)new ParseError($"I don't understand '{tokens[0]}'. Type 'help' for available commands.")
            : parser(tokens[1..]);
    }
}
