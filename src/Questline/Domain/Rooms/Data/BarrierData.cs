namespace Questline.Domain.Rooms.Data;

public class BarrierData
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string BlockedMessage { get; set; } = null!;
    public string UnlockItemId { get; set; } = null!;
    public string UnlockMessage { get; set; } = null!;
}
