namespace Questline.Framework.Mediator;

public interface IRequest
{
    static abstract IRequest CreateRequest(string[] args);
}

public interface IResponse
{
    string Message { get; }
}
