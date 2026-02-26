using Questline.Framework.Domain;

namespace Questline.Domain.Players.Entity;

public class Player : DomainEntity
{
    public string Username { get; init; } = null!;
    public string Name     { get; init; } = null!;

    public static Player Create(string id, string username, string name)
    {
        return new Player
        {
            Id       = id,
            Username = username,
            Name     = name
        };
    }
}
