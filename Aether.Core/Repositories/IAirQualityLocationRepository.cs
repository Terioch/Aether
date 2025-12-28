using Aether.Core.Models.DbQueries;

namespace Aether.Core.Repositories;

public interface IAirQualityLocationRepository
{
    Task<List<DbAirQualityItem>> GetAirQualityDataWithinBounds(GeoLocation northEast, GeoLocation southWest, double latStep, double lngStep);
}
