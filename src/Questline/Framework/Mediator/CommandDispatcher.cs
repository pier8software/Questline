using Microsoft.Extensions.DependencyInjection;
using Questline.Domain;
using Questline.Domain.Shared;
using Questline.Domain.Shared.Data;

namespace Questline.Framework.Mediator;

public class CommandDispatcher(IServiceProvider serviceProvider)
{
    public CommandResult Dispatch(GameState state, ICommand command)
    {
        var commandType = command.GetType();
        var commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
        var commandExecuteMethod = commandHandlerType.GetMethod(nameof(ICommandHandler<ICommand>.Execute))!;

        var handler = serviceProvider.GetRequiredService(commandHandlerType);

        return (CommandResult)commandExecuteMethod.Invoke(handler, [state, command])!;
    }
}
