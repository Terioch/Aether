namespace Aether.Core.Requests;
public class DashboardViewRequest
{
    public int? ReadingId { get; set; }

    public required GeoLocation Location { get; set; }
}
