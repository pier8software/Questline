using Questline.Domain.Characters;

namespace Questline.Tests.Domain.Characters;

public class When_name_is_invalid
{
    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("Abcdefghijklmnopqrstuvwxy")]
    [InlineData(" Thorin")]
    [InlineData("Thorin ")]
    public void Invalid_name_returns_failure(string name) =>
        CharacterNameValidator.HaveValidLength(name).ShouldBeFalse();
}
