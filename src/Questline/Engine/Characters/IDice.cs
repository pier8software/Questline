namespace Questline.Engine.Characters;

public interface IDice
{
    int Roll(int sides);
    int Roll(int count, int sides);
}

public sealed class Dice : IDice
{
    public int Roll(int sides)
    {
        if (sides < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(sides), "Sides must be at least 1.");
        }

        return Random.Shared.Next(1, sides + 1);
    }

    public int Roll(int count, int sides)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be at least 1.");
        }

        if (sides < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(sides), "Sides must be at least 1.");
        }

        var total = 0;
        for (var i = 0; i < count; i++)
        {
            total += Roll(sides);
        }
        return total;
    }
}
