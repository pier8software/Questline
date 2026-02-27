using Microsoft.Extensions.DependencyInjection;

namespace Questline.Framework.Mediator;

public class RequestSender(IServiceProvider serviceProvider)
{
    public async Task<IResponse> Send(IRequest request)
    {
        var requestType = request.GetType();

        var requestHandlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        var handleMethod       = requestHandlerType.GetMethod(nameof(IRequestHandler<>.Handle))!;

        var handler = serviceProvider.GetRequiredService(requestHandlerType);

        var task = (Task<IResponse>)handleMethod.Invoke(handler, [request])!;
        return await task;
    }
}
