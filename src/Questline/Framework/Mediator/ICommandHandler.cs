using Questline.Domain.Shared.Data;

namespace Questline.Framework.Mediator;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    CommandResult Execute(GameState state, TCommand command);
}
