using System.Reflection;
using Questline.Domain.Characters;
using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Engine.Characters.PartyGeneration;
using Questline.Engine.Messages;
using Questline.Engine.Parsers;
using Questline.Framework.Mediator;

namespace Questline.Engine.Characters;

public class PartyCreationStateMachine(IDice dice)
{
    private const int PartySize = 4;

    private static readonly Race[] AllRaces =
        [Race.Human, Race.Dwarf, Race.Elf, Race.Halfling];

    private Party? _party;

    public Party? CompletedParty { get; private set; }

    public IResponse Start()
    {
        _party = RollParty();
        return BuildRolledResponse();
    }

    public IResponse ProcessInput(string? input)
    {
        if (_party is null)
        {
            return Start();
        }

        var trimmed = input?.Trim() ?? "";

        if (string.Equals(trimmed, "accept", StringComparison.OrdinalIgnoreCase))
        {
            CompletedParty = _party;
            return new Responses.PartyAcceptedResponse();
        }

        if (string.Equals(trimmed, "reroll", StringComparison.OrdinalIgnoreCase))
        {
            _party = RollParty();
            return BuildRolledResponse();
        }

        if (trimmed.StartsWith("rename ", StringComparison.OrdinalIgnoreCase))
        {
            return HandleRename(trimmed["rename ".Length..]);
        }

        return new Responses.PartyCreationErrorResponse(
            "Type 'accept', 'reroll', or 'rename <slot|name> <newName>'.");
    }

    private IResponse HandleRename(string args)
    {
        var parts = args.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return new Responses.PartyCreationErrorResponse(
                "Usage: rename <slot|name> <newName>");
        }

        var target  = parts[0];
        var newName = parts[1];

        if (!IsValidName(newName, _party!))
        {
            return new Responses.PartyCreationErrorResponse(
                $"Invalid name '{newName}': must be a single token, unique, and not a reserved verb.");
        }

        var index = ResolveTargetIndex(target);
        if (index is null)
        {
            return new Responses.PartyCreationErrorResponse($"No character matches '{target}'.");
        }

        _party = ReplaceMemberName(_party!, index.Value, newName);
        return BuildRolledResponse();
    }

    private int? ResolveTargetIndex(string target)
    {
        if (int.TryParse(target, out var slot) && slot is >= 1 and <= PartySize)
        {
            return slot - 1;
        }

        for (var i = 0; i < _party!.Members.Count; i++)
        {
            if (_party.Members[i].Name.Equals(target, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }
        return null;
    }

    private static Party ReplaceMemberName(Party party, int index, string newName)
    {
        var members = party.Members.ToList();
        var old     = members[index];
        var renamed = Character.Create(
            id: old.Id,
            name: newName,
            race: old.Race,
            characterClass: old.Class,
            hitPoints: old.HitPoints,
            abilityScores: old.AbilityScores,
            occupation: old.Occupation,
            level: old.Level);

        foreach (var item in old.Inventory)
        {
            renamed.AddInventoryItem(item);
        }

        members[index] = renamed;
        return new Party(party.Id, members);
    }

    private Party RollParty()
    {
        var members = new List<Character>(PartySize);

        for (var i = 0; i < PartySize; i++)
        {
            var race          = AllRaces[dice.Roll(AllRaces.Length) - 1];
            var abilityScores = AbilityScoresCalculator.Calculate(dice);
            var hp            = dice.Roll(1, 4);
            var hitPoints     = new HitPoints(max: hp, current: hp);
            var occupation    = Occupations.Pick(dice);

            string name;
            do
            {
                name = Names.Pick(race, dice);
            }
            while (NameClashes(name, members));

            members.Add(Character.Create(
                id: Guid.NewGuid().ToString(),
                name: name,
                race: race,
                characterClass: null,
                hitPoints: hitPoints,
                abilityScores: abilityScores,
                occupation: occupation,
                level: 0));
        }

        return new Party(id: Guid.NewGuid().ToString(), members: members);
    }

    private bool IsValidName(string name, Party currentParty)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        if (name.Contains(' ')) return false;
        if (ReservedVerbs.Contains(name.ToLowerInvariant())) return false;
        if (currentParty.Members.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return false;
        return true;
    }

    private static bool NameClashes(string name, IReadOnlyList<Character> members)
    {
        if (ReservedVerbs.Contains(name.ToLowerInvariant())) return true;
        return members.Any(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    private static readonly HashSet<string> ReservedVerbs = BuildReservedVerbs();

    private static HashSet<string> BuildReservedVerbs()
    {
        var verbs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var type in typeof(Requests).Assembly.GetTypes())
        {
            var attr = type.GetCustomAttribute<VerbsAttribute>();
            if (attr is null) continue;
            foreach (var v in attr.Verbs) verbs.Add(v);
        }
        return verbs;
    }

    private IResponse BuildRolledResponse() =>
        new Responses.PartyRolledResponse(
            _party!.Members.Select(m => m.ToSummary()).ToList());
}
