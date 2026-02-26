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
    private          Character?               _completedCharacter;

    public CharacterCreationStateMachine(IDice dice)
    {
        _dice    = dice;
        _context = InitializeCharacter();
    }

    public Character? CompletedCharacter => _completedCharacter;

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
            return new Responses.CharacterCreationResponse(
                Responses.CharacterCreationStep.SelectClass,
                "Select your character's class:",
                [new Responses.CharacterCreationOption("1", "Fighter")]);
        }

        _context.State = CharacterCreationState.PendingRaceSelection;

        return new Responses.CharacterCreationResponse(
            Responses.CharacterCreationStep.SelectRace,
            "Select your character's race:",
            [new Responses.CharacterCreationOption("1", "Human")]);
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
            return new Responses.CharacterCreationResponse(
                Responses.CharacterCreationStep.SelectRace,
                "Select your character's race:",
                [new Responses.CharacterCreationOption("1", "Human")]);
        }

        _context.State = CharacterCreationState.PendingHitPoints;

        return new Responses.CharacterCreationResponse(
            Responses.CharacterCreationStep.RollHitPoints,
            "Hit enter to continue.");
    }

    private IResponse ProcessHitPoints()
    {
        _context.HitPoints = HitPointsCalculator.Calculate(_context.Class, _dice);

        _context.State = CharacterCreationState.PendingCharacterName;

        return new Responses.CharacterCreationResponse(
            Responses.CharacterCreationStep.EnterName,
            "Enter a name for your character");
    }

    private IResponse ProcessCharacterName(string? input)
    {
        var characterName = new CharacterName(input);

        var validationResult = CharacterNameValidator.Instance.Validate(characterName);
        if (!validationResult.IsValid)
        {
            return new Responses.CharacterCreationResponse(
                Responses.CharacterCreationStep.EnterName,
                validationResult.Errors.First().ErrorMessage);
        }

        _context.Name = characterName.Name!;

        _completedCharacter = Character.Create(
            _context.Name,
            _context.Race,
            _context.Class,
            _context.HitPoints,
            _context.AbilityScores);

        _context.State = CharacterCreationState.Complete;

        return new Responses.CharacterCreationCompleteResponse(_completedCharacter.ToSummary());
    }
}
