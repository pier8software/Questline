using Questline.Domain.Characters.Entity;
using Questline.Engine.Characters;
using Questline.Engine.Core;
using Questline.Engine.Messages;

namespace Questline.Cli;

public class CliApp(
    IConsole console,
    CharacterCreationStateMachine stateMachine,
    GameEngine engine)
{
    public void Run()
    {
        DisplayWelcomeMessage();

        var character = InitiateCharacterSetup();

        LaunchGameSession(character);

        HandleGameLoop();
    }

    private void HandleGameLoop()
    {
        while (true)
        {
            console.Write("> ");
            var input = console.ReadLine();

            var result = engine.ProcessInput(input);

            console.WriteLine(result.Message);

            if (result is Responses.GameQuited)
            {
                break;
            }
        }
    }

    private void LaunchGameSession(Character character)
    {
        var message = engine.LaunchGame(character, "the-goblins-lair");
        console.WriteLine(message.Message);
    }

    private Character InitiateCharacterSetup()
    {
        console.WriteLine("Hit enter to create a new character...");
        var input = console.ReadLine();
        while (true)
        {
            var response = stateMachine.ProcessInput(input);
            console.WriteLine(response.Message);

            if (response is Responses.CharacterCreationCompleteResponse characterResponse)
            {
                var character = characterResponse.Character;
                console.WriteLine(character.ToCharacterSummary());
                return character;
            }

            input = console.ReadLine();
        }
    }

    private void DisplayWelcomeMessage() => console.WriteLine("Welcome to Questline!");
}
