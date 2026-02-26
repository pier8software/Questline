using Questline.Domain.Characters;
using Questline.Domain.Characters.Data;
using Questline.Domain.Characters.Entity;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Characters;

public class CharacterCreationContext
{
    public CharacterCreationState State { get; set; } = CharacterCreationState.PendingAbilityScores;

    public string          Name          { get; set; } = null!;
    public Race?           Race          { get; set; }
    public CharacterClass? Class         { get; set; }
    public HitPoints       HitPoints     { get; set; }  = null!;
    public AbilityScores   AbilityScores { get; init; } = null!;
}

public enum CharacterCreationState
{
    PendingAbilityScores,
    PendingClassSelection,
    PendingRaceSelection,
    PendingHitPoints,
    PendingCharacterName,
    Complete
}

public class CharacterCreationStateMachine
{
    private readonly CharacterCreationContext _context;
    private readonly IDice                    _dice;

    public CharacterCreationStateMachine(IDice dice)
    {
        _dice    = dice;
        _context = InitializeCharacter();
    }

    public IResponse ProcessInput(string? input)
    {
        return _context.State switch
        {
            CharacterCreationState.PendingClassSelection => ProcessClassSelection(input),
            CharacterCreationState.PendingRaceSelection  => ProcessRaceSelection(input),
            CharacterCreationState.PendingHitPoints      => ProcessHitPoints(),
            CharacterCreationState.PendingCharacterName  => ProcessCharacterName(input),

            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private CharacterCreationContext InitializeCharacter() =>
        new()
        {
            State         = CharacterCreationState.PendingClassSelection,
            AbilityScores = AbilityScoresCalculator.Calculate(_dice)
        };

    private IResponse ProcessClassSelection(string? input)
    {
        _context.Class = input?.ToLower() switch
        {
            "1" or "fighter" => CharacterClass.Fighter,
            _                => null
        };

        if (_context.Class == null)
        {
            return new Responses.CharacterCreationResponse(Prompts.SelectClass);
        }

        _context.State = CharacterCreationState.PendingRaceSelection;

        return new Responses.CharacterCreationResponse(Prompts.SelectRace);
    }

    private IResponse ProcessRaceSelection(string? input)
    {
        _context.Race = input?.ToLower() switch
        {
            "1" or "human" => Race.Human,
            _              => null
        };

        if (_context.Race == null)
        {
            return new Responses.CharacterCreationResponse(Prompts.SelectRace);
        }

        _context.State = CharacterCreationState.PendingHitPoints;

        return new Responses.CharacterCreationResponse(Prompts.Continue);
    }

    private IResponse ProcessHitPoints()
    {
        _context.HitPoints = HitPointsCalculator.Calculate(_context.Class, _dice);

        _context.State = CharacterCreationState.PendingCharacterName;

        return new Responses.CharacterCreationResponse(Prompts.EnterName);
    }

    private IResponse ProcessCharacterName(string? input)
    {
        var characterName = new CharacterName(input);

        var validationResult = CharacterNameValidator.Instance.Validate(characterName);
        if (!validationResult.IsValid)
        {
            return new Responses.CharacterCreationResponse(validationResult.Errors.First().ErrorMessage);
        }

        _context.Name = characterName.Name!;

        var character = Character.Create(
            _context.Name,
            _context.Race,
            _context.Class,
            _context.HitPoints,
            _context.AbilityScores);

        _context.State = CharacterCreationState.Complete;

        return new Responses.CharacterCreationCompleteResponse(Prompts.CharacterCreated, character);
    }

    private static class Prompts
    {
        public const string SelectClass      = "Select your character's class:\n\t1. Fighter";
        public const string SelectRace       = "Select your character's race:\n\t1. Human";
        public const string EnterName        = "Enter a name for your character";
        public const string Continue         = "Hit enter to continue.";
        public const string CharacterCreated = "You have created your character.";
    }
}
