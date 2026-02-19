namespace Questline.Engine.Characters;

public interface IDice
{
    int[] Roll(int diceAmount, int sides);
}

public class Dice : IDice
{
    public int[] Roll(int diceAmount, int sides) =>
        Enumerable.Range(0, diceAmount).Select(_ => Random.Shared.Next(1, sides + 1)).ToArray();
}
