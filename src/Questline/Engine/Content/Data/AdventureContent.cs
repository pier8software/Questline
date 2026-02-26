using Questline.Domain.Rooms.Entity;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Engine.Content.Data;

public record AdventureContent(
    Dictionary<string, Room> Rooms,
    Dictionary<string, Barrier> Barriers,
    string StartingRoomId);
