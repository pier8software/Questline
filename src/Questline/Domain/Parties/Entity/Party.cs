using Questline.Domain.Characters.Entity;
using Questline.Framework.Domain;

namespace Questline.Domain.Parties.Entity;

public class Party : DomainEntity
{
    public Party(string id, IReadOnlyList<Character> members)
    {
        Id      = id;
        Members = members;
    }

    public IReadOnlyList<Character> Members { get; }

    public IReadOnlyList<Character> MembersAlive =>
        Members.Where(c => c.HitPoints.IsAlive).ToList();

    public Character? FindByName(string name) =>
        Members.FirstOrDefault(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public bool IsMember(Character character) =>
        Members.Any(c => c.Id == character.Id);
}
