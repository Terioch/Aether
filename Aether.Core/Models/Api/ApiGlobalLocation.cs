namespace Aether.Core.Models.Api;

public class ApiGlobalLocation
{
    public required string Name { get; set; }

    public required Dictionary<string, string> LocalNames { get; set; }

    public required string Latitude { get; set; }

    public required string Longitude { get; set; }

    public required string State { get; set; }

    public required string Country { get; set; }
}
