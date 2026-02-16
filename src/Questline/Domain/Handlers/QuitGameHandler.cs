using Questline.Domain.Messages;
using Questline.Domain.Shared.Data;
using Questline.Framework.Mediator;

namespace Questline.Domain.Handlers;

public class QuitGameHandler : ICommandHandler<Commands.QuitGame>
{
    public CommandResult Execute(GameState state, Commands.QuitGame game) => new Results.GameQuited();
}
