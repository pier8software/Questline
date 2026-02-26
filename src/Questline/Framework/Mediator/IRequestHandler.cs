using Questline.Engine.Core;

namespace Questline.Framework.Mediator;

public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
{
    IResponse Handle(GameState state, TRequest request);
}
