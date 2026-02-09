using Questline.Domain;

namespace Questline.Engine;

public class CommandDispatcher
{
    private readonly Dictionary<string, Registration> _registrations = new(StringComparer.OrdinalIgnoreCase);

    public void Register<TCommand>(string verb, ICommandHandler<TCommand> handler, Func<string[], TCommand?> factory)
        where TCommand : ICommand =>
        Register([verb], handler, factory);

    public void Register<TCommand>(string[] verbs, ICommandHandler<TCommand> handler, Func<string[], TCommand?> factory)
        where TCommand : ICommand
    {
        var registration = new Registration((state, args) =>
        {
            var command = factory(args);
            if (command is null)
            {
                return new ErrorResult("Invalid command arguments.");
            }

            return handler.Execute(state, command);
        });

        foreach (var verb in verbs)
        {
            _registrations[verb] = registration;
        }
    }

    public CommandResult Dispatch(GameState state, ParsedInput input)
    {
        if (string.IsNullOrEmpty(input.Verb))
        {
            return new ErrorResult("Please enter a command.");
        }

        if (!_registrations.TryGetValue(input.Verb, out var registration))
        {
            return new ErrorResult($"I don't understand '{input.Verb}'. Type 'help' for available commands.");
        }

        return registration.Execute(state, input.Args);
    }

    private record Registration(Func<GameState, string[], CommandResult> Execute);
}
