using System.Reflection;
using Questline.Framework.Mediator;

namespace Questline.Engine.Parsers;

public class Parser
{
    private readonly Dictionary<string, Func<string[], ParseResult>> _verbToParsers =
        new(StringComparer.OrdinalIgnoreCase);

    public Parser()
    {
        var requestTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IRequest).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });


        foreach (var type in requestTypes)
        {
            var attr = type.GetCustomAttribute<VerbsAttribute>();
            if (attr == null)
            {
                continue;
            }

            var createMethod = type.GetMethod(
                "CreateRequest",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy
            );

            if (createMethod == null)
            {
                continue;
            }

            var localMethod = createMethod;
            Func<string[], ParseResult> parser = args =>
            {
                try
                {
                    return localMethod.Invoke(null, [args]) is IRequest request
                        ? ParseResult.Success(request)
                        : ParseResult.Failure(new ParseError("Failed to create request."));
                }
                catch (TargetInvocationException ex)
                {
                    // Unwrap the actual exception
                    var innerException = ex.InnerException ?? ex;
                    return ParseResult.Failure(new ParseError($"Invalid arguments: {innerException.Message}"));
                }
                catch (Exception ex)
                {
                    return ParseResult.Failure(new ParseError($"Error parsing command: {ex.Message}"));
                }
            };

            foreach (var verb in attr.Verbs)
            {
                _verbToParsers[verb] = parser;
            }
        }
    }

    public ParseResult Parse(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return ParseResult.Failure(new ParseError("Please enter a command."));
        }

        var tokens = input.Trim().ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length == 0)
        {
            return ParseResult.Failure(new ParseError("Please enter a command."));
        }

        if (_verbToParsers.TryGetValue(tokens[0], out var parser))
        {
            return parser(tokens[1..]);
        }

        return ParseResult.Failure(new ParseError($"I don't understand '{tokens[0]}'."));
    }
}
