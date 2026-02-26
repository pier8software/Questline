namespace Questline.Domain.Rooms.Data;

public class FeatureData
{
    public string   Id          { get; set; } = null!;
    public string   Name        { get; set; } = null!;
    public string[] Keywords    { get; set; } = [];
    public string   Description { get; set; } = null!;
}
