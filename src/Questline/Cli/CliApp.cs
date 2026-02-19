using Questline.Domain.Characters;
using Questline.Domain.Players.Entity;
using Questline.Domain.Shared.Data;
using Questline.Engine.Characters;
using Questline.Engine.Content;
using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;

namespace Questline.Cli;

public class CliApp(IConsole console, WorldContent world, CharacterFactory characterFactory, Parser parser, RequestSender dispatcher)
{
    public void Run()
    {
        var character = PromptForCharacter();
        if (character is null)
        {
            return;
        }

        var player = new Player
        {
            Id = Guid.NewGuid().ToString(),
            Character = character,
            Location = world.StartingRoomId
        };

        var state = new GameState(world.Rooms, player, world.Barriers);
        var engine = new GameEngine(parser, dispatcher, state);

        console.WriteLine($"Welcome, {character.Name}! Your adventure begins...");

        var initialRoom = engine.ProcessInput("look");
        console.WriteLine(initialRoom.Message);

        while (true)
        {
            console.Write("> ");
            var input = console.ReadLine();

            if (input is null)
            {
                break;
            }

            var result = engine.ProcessInput(input);

            console.WriteLine(result.Message);

            if (result is Responses.GameQuited)
            {
                break;
            }
        }
    }

    private Domain.Characters.Entity.Character? PromptForCharacter()
    {
        while (true)
        {
            console.WriteLine("What is your name, adventurer?");
            console.Write("> ");
            var name = console.ReadLine();

            if (name is null)
            {
                return null;
            }

            if (!CharacterNameValidator.Validate(name))
            {
                console.WriteLine("Please give your character a name");
                continue;
            }

            return characterFactory.Create(name);
        }
    }
}
