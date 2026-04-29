namespace Questline.Framework.Mediator;

public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
{
    Task<IResponse> Handle(Actor actor, TRequest request);
}
