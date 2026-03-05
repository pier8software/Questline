namespace Questline.Tests.Domain.Characters.CharacterNameValidator;

public class When_name_is_valid
{
    [Theory]
    [InlineData("Thorin")]
    [InlineData("Sir Lancelot")]
    [InlineData("Bo")]
    [InlineData("Abcdefghijklmnopqrstuvwx")]
    public void Valid_name_returns_success(string name) =>
        Questline.Domain.Characters.CharacterNameValidator.HaveValidLength(name).ShouldBeTrue();
}
