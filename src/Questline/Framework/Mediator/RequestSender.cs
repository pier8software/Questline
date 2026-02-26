using Microsoft.Extensions.DependencyInjection;
using Questline.Domain.Shared.Data;
using Questline.Engine.Core;

namespace Questline.Framework.Mediator;

public class RequestSender(IServiceProvider serviceProvider)
{
    public IResponse Send(GameState? state, IRequest request)
    {
        var requestType = request.GetType();

        var requestHandlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        var handleMethod       = requestHandlerType.GetMethod(nameof(IRequestHandler<>.Handle))!;

        var handler = serviceProvider.GetRequiredService(requestHandlerType);

        return (IResponse)handleMethod.Invoke(handler, [state, request])!;
    }
}
