using Questline.Domain;
using Questline.Engine.Commands;

namespace Questline.Engine.Handlers;

public class QuitCommandHandler : ICommandHandler<QuitCommand>
{
    public CommandResult Execute(GameState state, QuitCommand command) => new QuitResult();
}
