using Questline;

var state = new GameState();
var parser = new CommandParser();
var handler = new CommandHandler();

Console.WriteLine("Welcome to Questline!");
Console.WriteLine();
Console.WriteLine(handler.DescribeRoom(state));

while (true)
{
    Console.WriteLine();
    Console.Write("> ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    var command = parser.Parse(input);
    var result = handler.Execute(command, state);

    Console.WriteLine(result.Output);
    state = result.NewState;

    if (state.Flags.GetValueOrDefault("quit"))
    {
        break;
    }
}
