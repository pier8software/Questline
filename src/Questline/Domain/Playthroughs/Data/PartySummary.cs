using Questline.Domain.Characters.Data;

namespace Questline.Domain.Playthroughs.Data;

public record PartySummary(IReadOnlyList<CharacterSummary> Members, int Turns);
