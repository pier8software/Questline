using Questline.Domain.Characters;

namespace Questline.Tests.Domain.Characters;

public class CharacterNameValidatorTests
{
    [Theory]
    [InlineData("Thorin")]
    [InlineData("Sir Lancelot")]
    [InlineData("Bo")]
    [InlineData("Abcdefghijklmnopqrstuvwx")]
    public void Valid_name_returns_success(string name) =>
        CharacterNameValidator.HaveValidLength(name).ShouldBeTrue();

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("Abcdefghijklmnopqrstuvwxy")]
    [InlineData(" Thorin")]
    [InlineData("Thorin ")]
    public void Invalid_name_returns_failure(string name) =>
        CharacterNameValidator.HaveValidLength(name).ShouldBeFalse();
}
