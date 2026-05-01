namespace Questline.Domain.Characters.Entity;

public class HitPoints
{
    public HitPoints(int max, int current)
    {
        Max     = max;
        Current = current;
    }

    public int Max     { get; }
    public int Current { get; private set; }

    public bool IsAlive => Current > 0;

    public void Damage(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Damage amount must be non-negative.");
        }

        Current = Math.Max(0, Current - amount);
    }

    public void Heal(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Heal amount must be non-negative.");
        }

        Current = Math.Min(Max, Current + amount);
    }
}

public record AbilityScores(
    AbilityScore Strength,
    AbilityScore Intelligence,
    AbilityScore Wisdom,
    AbilityScore Dexterity,
    AbilityScore Constitution,
    AbilityScore Charisma);

public record AbilityScore(int Score)
{
    public int Modifier => (int)Math.Floor((Score - 10) / 2.0);
}
