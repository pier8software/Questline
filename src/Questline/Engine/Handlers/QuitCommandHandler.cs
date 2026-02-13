using Questline.Domain;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class QuitCommandHandler : ICommandHandler<Commands.QuitCommand>
{
    public CommandResult Execute(GameState state, Commands.QuitCommand command) => new Results.QuitResult();
}
