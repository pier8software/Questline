using Questline.Domain.Shared.Data;

namespace Questline.Framework.Mediator;

public interface IRequestHandler<in TRequest, out TResponse>
    where TRequest : IRequest
    where TResponse : IResponse
{
    TResponse Handle(GameState state, TRequest request);
}
