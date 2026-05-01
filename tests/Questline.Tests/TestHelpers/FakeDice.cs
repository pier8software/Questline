using Questline.Engine.Characters;

namespace Questline.Tests.TestHelpers;

public class FakeDice(params int[] results) : IDice
{
    private int _index;

    public int Roll(int sides) =>
        results[_index++];

    public int Roll(int count, int sides)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        var total = 0;
        for (var i = 0; i < count; i++)
        {
            total += results[_index++];
        }
        return total;
    }
}
