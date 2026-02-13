using Questline.Domain;

namespace Questline.Tests.Domain;

public class WorldTests
{
    [Fact]
    public void Throws_when_room_not_found()
    {
        var world = new WorldBuilder()
            .WithRoom("only-room", "Only Room", "The only room.")
            .Build();

        Should.Throw<KeyNotFoundException>(() => world.GetRoom("nonexistent"));
    }
}
