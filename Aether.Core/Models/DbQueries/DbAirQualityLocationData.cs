using Aether.Core.Entities;

namespace Aether.Core.Models.DbQueries;

public class DbAirQualityLocationData
{
    public required List<DbAirQualityItem> Readings { get; set; }

    public required List<LocationEntity> MissingLocations { get; set; }
}

public class DbAirQualityItem 
{
    public int LocationId { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public int? ReadingId { get; set; }

    public int Index { get; set; }

    public double SulfurDioxide { get; set; }

    public double NitrogenOxide { get; set; }

    public double NitrogenDioxide { get; set; }

    public double ParticulateMatter10 { get; set; }

    public double ParticulateMatter2_5 { get; set; }

    public double Ozone { get; set; }

    public double CarbonMonoxide { get; set; }

    public double Ammonia { get; set; }
}