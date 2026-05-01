using Questline.Domain.Characters.Entity;

namespace Questline.Framework.Mediator;

public abstract record Actor;

public sealed record NoActor : Actor;

public sealed record PartyActor : Actor;

public sealed record CharacterActor(Character Character) : Actor;
