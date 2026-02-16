using Questline.Domain;
using Questline.Domain.Rooms.Entity;

namespace Questline.Engine.InputParsers;

public static class DirectionParser
{
    private static readonly Dictionary<string, Direction> Directions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["north"] = Direction.North,
        ["n"] = Direction.North,
        ["south"] = Direction.South,
        ["s"] = Direction.South,
        ["east"] = Direction.East,
        ["e"] = Direction.East,
        ["west"] = Direction.West,
        ["w"] = Direction.West,
        ["up"] = Direction.Up,
        ["u"] = Direction.Up,
        ["down"] = Direction.Down,
        ["d"] = Direction.Down
    };

    public static bool TryParse(string input, out Direction direction) => Directions.TryGetValue(input, out direction);
}
