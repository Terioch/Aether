using Aether.Core.Models.DbQueries;

namespace Aether.Core.Repositories;

public interface IAirQualityLocationRepository
{
    Task<DbAirQualityLocationData> GetAirQualityDataWithinBounds(GeoLocation northEast, GeoLocation southWest);
}
