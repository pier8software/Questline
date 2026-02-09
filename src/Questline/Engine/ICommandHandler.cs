using Questline.Domain;

namespace Questline.Engine;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    CommandResult Execute(GameState state, TCommand command);
}
