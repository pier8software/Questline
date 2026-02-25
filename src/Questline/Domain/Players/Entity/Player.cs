using Questline.Framework.Domain;

namespace Questline.Domain.Players.Entity;

public class Player : DomainEntity
{
    public string Name { get; set; }

    private Player(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Player Create(string name)
    {
        var id = Guid.NewGuid().ToString();
        return new Player(id, name);
    }
}
