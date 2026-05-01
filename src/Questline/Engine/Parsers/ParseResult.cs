using Questline.Framework.Mediator;

namespace Questline.Engine.Parsers;

public class ParseResult
{
    private ParseResult(IRequest? request, Actor? actor, ParseError? error, bool isSuccess)
    {
        Request   = request;
        Actor     = actor;
        Error     = error;
        IsSuccess = isSuccess;
    }

    public bool        IsSuccess { get; }
    public Actor?      Actor     { get; }
    public IRequest?   Request   { get; }
    public ParseError? Error     { get; }

    public static ParseResult Success(IRequest request, Actor? actor = null) =>
        new(request, actor, null, true);

    public static ParseResult Failure(ParseError error) =>
        new(null, null, error, false);
}
