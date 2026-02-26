namespace Questline.Framework.Mediator;

public interface IRequest
{
    static abstract IRequest CreateRequest(string[] args);
}
