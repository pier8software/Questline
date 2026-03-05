namespace Questline.Tests.TestHelpers.Builders.Templates;

public static partial class Templates
{
    public static class Rooms
    {
        public static RoomBuilder Cellar =>
            new RoomBuilder()
                .WithId("cellar")
                .WithName("Cellar")
                .WithDescription("A damp cellar.");

        public static RoomBuilder Chamber =>
            new RoomBuilder()
                .WithId("chamber")
                .WithName("Chamber")
                .WithDescription("A dark chamber.");

        public static RoomBuilder Hallway =>
            new RoomBuilder()
                .WithId("hallway")
                .WithName("Hallway")
                .WithDescription("A long hallway.");

        public static RoomBuilder Entrance =>
            new RoomBuilder()
                .WithId("entrance")
                .WithName("Entrance")
                .WithDescription("The entrance.");

        public static RoomBuilder StartRoom =>
            new RoomBuilder()
                .WithId("start")
                .WithName("Start")
                .WithDescription("Starting room.");

        public static RoomBuilder EndRoom =>
            new RoomBuilder()
                .WithId("end")
                .WithName("End Room")
                .WithDescription("The end room.");

        public static RoomBuilder ThroneRoom =>
            new RoomBuilder()
                .WithId("throne-room")
                .WithName("Throne Room")
                .WithDescription("Grand throne room.");

        public static RoomBuilder BeyondRoom =>
            new RoomBuilder()
                .WithId("beyond")
                .WithName("Beyond")
                .WithDescription("Beyond the door.");

        public static RoomBuilder DungeonEntrance =>
            new RoomBuilder()
                .WithId("entrance")
                .WithName("Dungeon Entrance")
                .WithDescription("A dark entrance.");

        public static RoomBuilder SealedRoom =>
            new RoomBuilder()
                .WithId("sealed")
                .WithName("Sealed Room")
                .WithDescription("No way north.");
    }
}
