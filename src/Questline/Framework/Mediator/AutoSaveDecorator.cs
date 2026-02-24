using Questline.Domain.Shared.Data;
using Questline.Framework.Persistence;

namespace Questline.Framework.Mediator;

public class AutoSaveDecorator<TRequest>(IRequestHandler<TRequest> inner, IGameStateRepository repository)
    : IRequestHandler<TRequest>
    where TRequest : IRequest
{
    public IResponse Handle(GameState state, TRequest request)
    {
        var response = inner.Handle(state, request);
        repository.Save(state);
        return response;
    }
}
