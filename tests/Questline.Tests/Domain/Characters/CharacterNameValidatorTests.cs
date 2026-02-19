using Questline.Domain.Characters;

namespace Questline.Tests.Domain.Characters;

public class CharacterNameValidatorTests
{
    [Fact]
    public void Valid_name_returns_success()
    {
        CharacterNameValidator.Validate("Thorin").ShouldBeTrue();
    }

    [Fact]
    public void Name_with_spaces_is_valid()
    {
        CharacterNameValidator.Validate("Sir Lancelot").ShouldBeTrue();
    }

    [Fact]
    public void Two_character_name_is_valid()
    {
        CharacterNameValidator.Validate("Bo").ShouldBeTrue();
    }

    [Fact]
    public void Twenty_four_character_name_is_valid()
    {
        CharacterNameValidator.Validate("Abcdefghijklmnopqrstuvwx").ShouldBeTrue();
    }

    [Fact]
    public void Empty_name_returns_failure()
    {
        CharacterNameValidator.Validate("").ShouldBeFalse();
    }

    [Fact]
    public void Single_character_name_returns_failure()
    {
        CharacterNameValidator.Validate("A").ShouldBeFalse();
    }

    [Fact]
    public void Name_longer_than_24_characters_returns_failure()
    {
        CharacterNameValidator.Validate("Abcdefghijklmnopqrstuvwxy").ShouldBeFalse();
    }

    [Fact]
    public void Name_with_special_characters_returns_failure()
    {
        CharacterNameValidator.Validate("Th@rin!").ShouldBeFalse();
    }

    [Fact]
    public void Name_with_leading_whitespace_returns_failure()
    {
        CharacterNameValidator.Validate(" Thorin").ShouldBeFalse();
    }

    [Fact]
    public void Name_with_trailing_whitespace_returns_failure()
    {
        CharacterNameValidator.Validate("Thorin ").ShouldBeFalse();
    }
}
