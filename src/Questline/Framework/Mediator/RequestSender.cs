using Microsoft.Extensions.DependencyInjection;
using Questline.Domain.Shared.Data;

namespace Questline.Framework.Mediator;

public class RequestSender(IServiceProvider serviceProvider)
{
    public IResponse Send(GameState state, IRequest request)
    {
        var commandType = request.GetType();
        var commandHandlerType = typeof(IRequestHandler<,>).MakeGenericType(commandType);
        var commandExecuteMethod = commandHandlerType.GetMethod(nameof(IRequestHandler<,>.Handle))!;

        var handler = serviceProvider.GetRequiredService(commandHandlerType);

        return (IResponse)commandExecuteMethod.Invoke(handler, [state, request])!;
    }
}
