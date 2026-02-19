using Questline.Engine.Characters;

namespace Questline.Tests.TestHelpers;

public class FakeDice(params int[] results) : IDice
{
    private int _index;

    public int[] Roll(int diceAmount, int sides)
    {
        var rolls = results[_index..(_index + diceAmount)];
        _index += diceAmount;
        return rolls;
    }
}
