using System.Reflection;
using Questline.Domain.Parties.Entity;
using Questline.Framework.Mediator;

namespace Questline.Engine.Parsers;

public class Parser
{
    private readonly Dictionary<string, (Func<string[], ParseResult> Build, bool RequiresActor)> _verbToParsers;

    public Parser() : this(BuildDefaultVerbDictionary())
    {
    }

    public Parser(Dictionary<string, (Func<string[], ParseResult> Build, bool RequiresActor)> verbToParsers)
    {
        _verbToParsers = verbToParsers;
    }

    private static Dictionary<string, (Func<string[], ParseResult> Build, bool RequiresActor)> BuildDefaultVerbDictionary()
    {
        var verbs = new Dictionary<string, (Func<string[], ParseResult> Build, bool RequiresActor)>(
            StringComparer.OrdinalIgnoreCase);

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

            var requiresActor = type.GetCustomAttribute<RequiresActorAttribute>() != null;

            var createMethod = type.GetMethod(
                "CreateRequest",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy
            );

            if (createMethod == null)
            {
                continue;
            }

            var localMethod = createMethod;
            Func<string[], ParseResult> build = args =>
            {
                try
                {
                    return localMethod.Invoke(null, [args]) is IRequest request
                        ? ParseResult.Success(request)
                        : ParseResult.Failure(new ParseError("Failed to create request."));
                }
                catch (TargetInvocationException ex)
                {
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
                verbs[verb] = (build, requiresActor);
            }
        }

        return verbs;
    }

    public ParseResult Parse(string? input) => Parse(input, party: null);

    public ParseResult Parse(string? input, Party? party)
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

        Actor actor = new PartyActor();
        var startIndex = 0;

        if (party is not null)
        {
            var match = party.FindByName(tokens[0]);
            if (match is not null)
            {
                actor      = new CharacterActor(match);
                startIndex = 1;
            }
        }

        if (startIndex >= tokens.Length)
        {
            return ParseResult.Failure(new ParseError("Who should do what?"));
        }

        if (_verbToParsers.TryGetValue(tokens[startIndex], out var entry))
        {
            var result = entry.Build(tokens[(startIndex + 1)..]);
            if (!result.IsSuccess)
            {
                return result;
            }

            if (entry.RequiresActor && actor is not CharacterActor)
            {
                return ParseResult.Failure(new ParseError("Which character should do that?"));
            }

            return ParseResult.Success(result.Request!, actor);
        }

        return ParseResult.Failure(new ParseError($"I don't understand '{tokens[startIndex]}'."));
    }
}
