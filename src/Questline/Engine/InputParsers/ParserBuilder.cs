using OneOf;
using Questline.Framework.Mediator;

namespace Questline.Engine.InputParsers;

public class ParserBuilder
{
    private readonly Dictionary<string, Func<string[], OneOf<ICommand, ParseError>>> _parsers = new(StringComparer.OrdinalIgnoreCase);

    public ParserBuilder RegisterCommand<TCommand>(string[] verbs, Func<string[], OneOf<ICommand, ParseError>> factory)
    {
        foreach (var verb in verbs)
        {
            _parsers[verb] = factory;
        }

        return this;
    }

    public Parser Build() => new(_parsers);
}
