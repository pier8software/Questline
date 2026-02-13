using Questline.Domain;
using Questline.Domain.Shared;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class QuitGameHandler : ICommandHandler<Commands.QuitGame>
{
    public CommandResult Execute(GameState state, Commands.QuitGame game) => new Results.GameQuited();
}
