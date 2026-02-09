namespace Questline.Domain;

public class GameState(World world, Player player)
{
    public World World { get; } = world;
    public Player Player { get; } = player;
}
