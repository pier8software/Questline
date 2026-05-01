using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Framework.Mediator;

namespace Questline.Engine.Core;

public static class ActingCharacterResolver
{
    public static Character Resolve(Actor actor, Party party) =>
        actor switch
        {
            CharacterActor ca => ca.Character,
            PartyActor _      => party.Members[0],
            NoActor _         => throw new InvalidOperationException(
                "NoActor cannot be resolved to a character."),
            _                 => throw new ArgumentOutOfRangeException(nameof(actor))
        };
}
