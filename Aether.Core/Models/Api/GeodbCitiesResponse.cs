namespace Aether.Core.Models.Api;

public class GeodbCitiesResponse
{
    public required List<GeodbLink> Links { get; set; }

    public required List<GeodbCity> Data { get; set; }
}

public class GeodbLink
{
    public required string Ref { get; set; }

    public required string Href { get; set; }
}

public class GeodbCity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Country { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public int Population { get; set; }
}
