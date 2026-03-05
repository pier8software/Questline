namespace Questline.Tests.Domain.Characters.CharacterNameValidator;

public class When_name_is_invalid
{
    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("Abcdefghijklmnopqrstuvwxy")]
    [InlineData(" Thorin")]
    [InlineData("Thorin ")]
    public void Invalid_name_returns_failure(string name) =>
        Questline.Domain.Characters.CharacterNameValidator.HaveValidLength(name).ShouldBeFalse();
}
