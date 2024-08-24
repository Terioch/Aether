namespace Aether.Core.Models;

public class MapEntry
{
    public required GeoLocation Location { get; set; }

    public required AirQualityIndex AirQualityIndex { get; set; }
}
