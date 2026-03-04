using Questline.Domain.Characters;

namespace Questline.Tests.Domain.Characters;

public class When_name_is_valid
{
    [Theory]
    [InlineData("Thorin")]
    [InlineData("Sir Lancelot")]
    [InlineData("Bo")]
    [InlineData("Abcdefghijklmnopqrstuvwx")]
    public void Valid_name_returns_success(string name) =>
        CharacterNameValidator.HaveValidLength(name).ShouldBeTrue();
}
