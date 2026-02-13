using Questline.Domain;
using Questline.Domain.Shared;

namespace Questline.Framework.Mediator;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    CommandResult Execute(GameState state, TCommand command);
}
