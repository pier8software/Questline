using Questline.Engine.Core;

namespace Questline.Framework.Mediator;

public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
{
    Task<IResponse> Handle(GameState state, TRequest request);
}
