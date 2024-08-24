namespace Aether.Core.Requests;

public sealed record GetNearbyMapEntriesRequest
{
    public required GeoLocation Location { get; set; }

    public int Zoom { get; set; }

    public required MapBounds Bounds { get; set; }
}

public sealed record MapBounds 
{
    public required GeoLocation NorthEast { get; set; }

    public required GeoLocation SouthWest { get; set; }
}
