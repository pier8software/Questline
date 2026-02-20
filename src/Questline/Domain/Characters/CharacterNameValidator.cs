using FluentValidation;
using Questline.Domain.Characters.Data;

namespace Questline.Domain.Characters;

public class CharacterNameValidator : AbstractValidator<CharacterName>
{
    public static CharacterNameValidator Instance { get; } = new();

    private CharacterNameValidator()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage("Please give your character a name.");
        RuleFor(c => c.Name).Must(HaveValidLength).WithMessage("Your character's name must be between 2 and 24 characters.");
    }

    public static bool HaveValidLength(string name) =>
        name.Length is >= 2 and <= 24
        && name == name.Trim();
}
