using Questline.Framework.Mediator;

namespace Questline.Engine.Parsers;

public class ParseResult
{
    public bool IsSuccess { get; }
    public IRequest? Request { get; }
    public ParseError? Error { get; }

    private ParseResult(IRequest? request, ParseError? error, bool isSuccess)
    {
        Request = request;
        Error = error;
        IsSuccess = isSuccess;
    }

    public static ParseResult Success(IRequest request) => new(request, null, true);
    public static ParseResult Failure(ParseError error) => new(null, error, false);
}
