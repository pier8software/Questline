namespace Questline.Engine.Characters;

public static class DiceExtensions
{
    public static CheckResult Check(this IDice dice, int modifier, int dc)
    {
        var roll = dice.Roll(20);
        return new CheckResult(roll, modifier, dc, roll + modifier >= dc);
    }
}

public record CheckResult(int Roll, int Modifier, int DC, bool Success);
