using System.Text.RegularExpressions;

namespace Questline.Domain.Characters;

public static partial class CharacterNameValidator
{
    public static bool Validate(string name) =>
        name.Length >= 2
        && name.Length <= 24
        && name == name.Trim()
        && AllowedPattern().IsMatch(name);

    [GeneratedRegex(@"^[a-zA-Z0-9 ]+$")]
    private static partial Regex AllowedPattern();
}
